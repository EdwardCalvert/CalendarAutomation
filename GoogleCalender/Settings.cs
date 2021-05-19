using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GoogleCalender;
using Newtonsoft.Json;
using System.Media;

namespace GoogleCalendar
{

    public struct SettingsStruct
    {
        public bool DebugMode;
        public bool Notifications;
        public bool LaunchURLs;
        public bool GoogleCalendarSync;
        public bool LaunchStartedURLsAfterReboot;
        public bool MonitorBrightnessControl;
        public Time StartWorkTime;
        public Time FinishWorkTime;
    }
    public class Settings
    {
        public SettingsStruct _settingsStruct;
        private FileSystemWatcher _fileSystemWatcher;
        private string _settingsName;
        private string _filePath;
        private SoundPlayer simpleSound = new SoundPlayer(Environment.CurrentDirectory + @"\Open.wav");


        public Settings(string settingsName, string filePath)
        {
            if (settingsName.EndsWith(".json"))
                _settingsName = settingsName;
            else
                _settingsName = settingsName + ".json";
            if (IsValidPath(filePath) && !filePath.EndsWith(".json"))
                _filePath = filePath;
            else
                throw new FileLoadException($"The given parameter, filePath {filePath} isn't considered a valid path.");
            InstantiateFileWatcher();
        }

        private bool IsValidPath(string path, bool allowRelativePaths = false)
        {
            bool isValid = true;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }

        public void ReadSettings()
        {
            //try
            //{
            if (File.Exists(GetFullFilePath()))
            {

                try
                {
                    using (StreamReader stream = new StreamReader(GetFullFilePath()))
                    {
                        using (JsonTextReader JsonReader = new JsonTextReader(stream))
                        {
                            var serializer = new JsonSerializer();
                            _settingsStruct = serializer.Deserialize<SettingsStruct>(JsonReader);
                        }

                    }
                }
                catch
                { 
                    //Corrupt File
                    SetDefaultSettings();
                    WriteSettings();
                }

            }
            else
            {
                SetDefaultSettings();
                WriteSettings();
                
            }

        }
        //    catch
        //    {
        //        
        //    }
        //}

        public void WriteSettings()
        {
            using (StreamWriter stream = new StreamWriter(GetFullFilePath()))
            {
                using (JsonTextWriter JsonWriter = new JsonTextWriter(stream))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(JsonWriter, _settingsStruct);
                }

            }
        }

        private string GetFullFilePath()
        {
            return _filePath + @"\" + _settingsName;
        }

        private void SetDefaultSettings()
        {
            _settingsStruct = new SettingsStruct();
            _settingsStruct.DebugMode = false;
            _settingsStruct.StartWorkTime = new Time(9, 0);
            _settingsStruct.FinishWorkTime = new Time(17, 0);
            _settingsStruct.LaunchStartedURLsAfterReboot = true;
            _settingsStruct.LaunchURLs = true;
            _settingsStruct.GoogleCalendarSync = true;
            _settingsStruct.Notifications = true;
        }

        private void InstantiateFileWatcher()
        {
            _fileSystemWatcher = new FileSystemWatcher(_filePath);
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _fileSystemWatcher.Filter = _settingsName;
            _fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            _fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            _fileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
            _fileSystemWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            simpleSound.Play();
            try
            {
                ReadSettings();
            }
            catch (Exception)
            {
                SetDefaultSettings();
            }
        }


    }

   
}
