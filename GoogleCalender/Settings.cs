using GoogleCalender;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GoogleCalendar
{
    public interface ISettings
    {
        void SetDefaultSettings();

    }

    public class  SettingsStruct : ISettings
    {
        public bool DebugMode;
        public bool Notifications;
        public string ProgramStartSound;
        public string LaunchURLNotification;
        public bool LaunchURLs;
        public bool GoogleCalendarSync;
        public bool LaunchStartedURLsAfterReboot;
        public bool MonitorBrightnessControl;
        public Time StartWorkTime;
        public Time FinishWorkTime;
        public bool IntellisenseForEvents;

        public  void SetDefaultSettings()
        {
            DebugMode = false;
            StartWorkTime = new Time(9, 0);
            FinishWorkTime = new Time(17, 0);
            LaunchStartedURLsAfterReboot = true;
            LaunchURLs = true;
            GoogleCalendarSync = true;
            Notifications = true;
            ProgramStartSound = "Exit Windows.wav";
            LaunchURLNotification = "Input.wav";
            IntellisenseForEvents = true;
        }
    }

    public class KeyPairSetting
    {
        public string Key;
        public string Value;
        
        public KeyPairSetting(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public class IntellisenseSettings : ISettings
    {
        public Dictionary<string,string> Values;

        public void SetDefaultSettings()
        {
            if (Values != null)
            {
                Values.Clear();
            }
            else
            {
                Values = new Dictionary<string, string>();
            }
            Values.Add("CalendarAutomation", @"https://trello.com/b/LouzIk2O/homework-automation-calendar-clock-v2");
        }
    }
    public class Settings< T> where T:ISettings
    {
        public  T settings;
        protected  FileSystemWatcher _fileSystemWatcher;
        protected string _settingsName;
        protected string _filePath;



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
            ReadSettings();
        }

        protected virtual bool IsValidPath(string path, bool allowRelativePaths = false)
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

        protected virtual bool SetToNullInstance()
        {
            return settings == null;
        }

        protected virtual void ReadSettings()
        {
            if (File.Exists(GetFullFilePath()))
            {

                try
                {
                    using (StreamReader stream = new StreamReader(GetFullFilePath()))
                    {
                        using (JsonTextReader JsonReader = new JsonTextReader(stream))
                        {
                            var serializer = new JsonSerializer();
                            settings = serializer.Deserialize<T>(JsonReader);
                        }

                    }
                    if(settings == null)
                    {
                        RestoreCorruptFile();
                    }
                    else
                    {
                        Notification.OpenSound();
                    }
                }
                catch
                {
                    //Corrupt File
                    RestoreCorruptFile();

                }

            }
            else
            {
                RestoreCorruptFile();

            }
            

        }

        private void RestoreCorruptFile()
        {
            settings = Activator.CreateInstance<T>();
            settings.SetDefaultSettings();
            WriteSettings();
            Notification.ProgramErrorSound();
        }

        public virtual void WriteSettings()
        {

            using (StreamWriter stream = new StreamWriter(GetFullFilePath()))
            {
                using (JsonTextWriter JsonWriter = new JsonTextWriter(stream))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(JsonWriter, settings);
                }

            }
        }

        protected virtual string GetFullFilePath()
        {
            return _filePath + @"\" + _settingsName;
        }

       

        protected virtual void InstantiateFileWatcher()
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

        protected virtual void OnChanged(object sender, FileSystemEventArgs e)
        {

            try
            {
                ReadSettings();
                //Notification.OpenSound();
            }
            catch (Exception)
            {
                settings.SetDefaultSettings();
                Notification.ProgramErrorSound();
            }
        }


    }


}
