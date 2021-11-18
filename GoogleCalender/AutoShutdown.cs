using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using Windows.Foundation.Collections;

//namespace GoogleCalender
//{
//    namespace AutoShutdown
//    //{

//        public class Alarm
//        {


//            Timer _timer;
//            private TimeSpan _timeToRing;

//            private int _numberOfRandomChars = 10;
//            private int _secondsToUnlock = 25;
//            private string _randomText;
//            private string _enteredText;
//            private string _action;
//            private int _attemptsMade = 0;
//            private int _lockoutAfterAttempts = 2;
//            private Timer _enforceSecondsToUnlock;
//            private ConfigFile ConfigFile1 = new ConfigFile("_numberOfRandomChars=[0-9]*;(\r\n|\r|\n)_timeToRing=([0-9]{2}:){2}([0-9]{2});(\r\n|\r|\n)_secondsToUnlock=[0-9]*;(\r\n|\r|\n)_lockoutAfterAttempts=[0-9]*;(\r\n|\r|\n)*", @"_numberOfRandomChars=10;
//_timeToRing=20:00:00;
//_secondsToUnlock=25;
//_lockoutAfterAttempts=2;");
//            private Stopwatch _stopwatch = new Stopwatch();
//            private readonly List<string> _randomChars = new List<string> { "!", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?", "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "[", "^", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "{", "|", "}", "~" };
//            Random random = new Random();

//            public Alarm()
//            {
//                string file = ConfigFile1.ReadConfigFile();
//                Console.WriteLine(file);
//                ParseConfigFile(file);
//                _timer = new Timer(_secondsToUnlock * 1000) { AutoReset = true };
//                _timer.Elapsed += TimerElapsed;


//                _enforceSecondsToUnlock = new Timer();
//                _enforceSecondsToUnlock.Interval = _secondsToUnlock * 1000;
//                _enforceSecondsToUnlock.Elapsed += ShutdownHandler;
//                _enforceSecondsToUnlock.AutoReset = false;

//                ToastNotificationManagerCompat.OnActivated += ToastHandler;
//                TimerElapsed(null, null);

//            }
//            private void TimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
//            {
//                Console.WriteLine("called");
//                Console.WriteLine(_timeToRing);
//                Console.WriteLine(DateTime.Now.TimeOfDay);
//                if (TimeSpan.Compare(_timeToRing, DateTime.Now.TimeOfDay) <= 0)
//                {
//                    BuildNotification(false);
//                }

//            }

//            private string GenerateRandomText(int length)
//            {
//                string text = "";
//                for (int i = 0; i < length; i++)
//                {
//                    text += _randomChars[random.Next(_randomChars.Count)];
//                }

//                return text;
//            }

//            private void BuildNotification(bool lastChance)
//            {

//                _randomText = GenerateRandomText(_numberOfRandomChars);
//                string title = lastChance ? $"Last chance to save PC {Environment.MachineName}!" : $"PC {Environment.MachineName} will shut down soon!";
//                new ToastContentBuilder()
//            .AddArgument("action", "viewConversation")
//            .AddText(title)
//            .AddText($"To prevent this, you have {_secondsToUnlock} seconds to type: \n {_randomText}")


//        // Text box for replying
//        .AddInputTextBox("tbUnlockCode", placeHolderContent: $"{_randomText}")

//        // Buttons
//        .AddButton(new ToastButton()
//            .SetContent("Shutdown")
//            .AddArgument("action", "Shutdown")
//            .SetBackgroundActivation())

//        .AddButton(new ToastButton()
//            .SetContent("Check")
//            .AddArgument("action", "Check")
//            .SetBackgroundActivation())
//        .Show();
//                _stopwatch.Reset();
//                _stopwatch.Start();

//                _enforceSecondsToUnlock.Start();
//            }

//            public void HandleNotificationFlow()
//            {

//                if (_action == "action=Check")
//                {
//                    _stopwatch.Stop();
//                    double secondsSinceSent = _stopwatch.ElapsedMilliseconds / 1000.0;
//                    if (_attemptsMade > _lockoutAfterAttempts || secondsSinceSent >= _secondsToUnlock)
//                    {
//                        Shutdown();
//                    }

//                    else if (_enteredText == _randomText) //Challenge correctly completed
//                    {
//                        _attemptsMade = 0;
//                        Console.WriteLine("Great success");
//                        _enforceSecondsToUnlock.Stop();

//                    }
//                    else if (_enteredText.Length > 0 || _enteredText != null) //Attempt made => final 
//                    {
//                        BuildNotification(true);
//                        _attemptsMade++;
//                    }

//                    else
//                    {
//                        Shutdown();
//                    }
//                }
//                else
//                {
//                    Shutdown();
//                }
//            }


//            private void ToastHandler(ToastNotificationActivatedEventArgsCompat toastArgs)
//            {
//                ValueSet userInput = toastArgs.UserInput;
//                _action = toastArgs.Argument;
//                _enteredText = userInput["tbUnlockCode"].ToString();

//                HandleNotificationFlow();

//            }

//            private void ShutdownHandler(Object source, System.Timers.ElapsedEventArgs e)
//            {
//                Shutdown();
//            }

//            private void Shutdown()
//            {

//                var psi = new ProcessStartInfo("shutdown", "/s /t 0");
//                psi.CreateNoWindow = true;
//                psi.UseShellExecute = false;
//                Process.Start(psi);
//                Console.Write("Would have shutdown!");
//            }

//            private void ParseConfigFile(string file)
//            {

//                string[] lines = file.Split('\n');
//                List<string> cheese = new List<string>(5);
//                foreach (string line in lines)
//                {
//                    int indexOfEquals = line.LastIndexOf("=") + 1;
//                    int indexOfSemiColon = line.LastIndexOf(";");
//                    cheese.Add(line.Substring(indexOfEquals, indexOfSemiColon - indexOfEquals));
//                }
//                _numberOfRandomChars = int.Parse(cheese[0]);
//                _timeToRing = TimeSpan.Parse(cheese[1]);
//                Console.WriteLine($"{cheese[1]}, this is the result {TimeSpan.Parse(cheese[1])}");
//                _secondsToUnlock = int.Parse(cheese[2]);
//                _lockoutAfterAttempts = int.Parse(cheese[3]);
//            }


//            public void Start() { _timer.Start(); }
//            public void Stop() { _timer.Stop(); }

//        }


//        public class ConfigFile
//        {
//            private string _configFileName = Environment.CurrentDirectory + "\\ConfigFile.txt";
//            private string _fileRegex;
//            private string _defaultFile;
//            public ConfigFile(string fileRegex, string defaultFile)
//            {
//                _fileRegex = fileRegex;
//                _defaultFile = defaultFile;

//            }

//            public bool ValidConfigFile(string file)
//            {
//                return Regex.IsMatch(file, _fileRegex);
//            }

//            public string ReadConfigFile()
//            {
//                string file = "";
//                if (File.Exists(_configFileName))
//                {
//                    using (StreamReader streamReader = new StreamReader(_configFileName))
//                    {
//                        file = streamReader.ReadToEnd();
//                    }

//                    if (ValidConfigFile(file))
//                    {
//                        return file;
//                    }

//                    else
//                    {
//                        return ResetFile();
//                    }
//                }

//                else
//                {
//                    return ResetFile();
//                }
//            }

//            private string ResetFile()
//            {
//                string file = "";
//                WriteFile(_defaultFile);
//                DisplayError("File was not in the expected format.");
//                //return ReadConfigFile();

//                using (StreamReader streamReader = new StreamReader(_configFileName))
//                {
//                    file = streamReader.ReadToEnd();
//                }
//                return file;
//            }

//            private void WriteFile(string fileContents)
//            {
//                using (StreamWriter stream = new StreamWriter(_configFileName))
//                {
//                    stream.Write(fileContents);
//                }
//            }


//            public void DisplayError(string error)
//            {
//                new ToastContentBuilder().AddText("Error While Reading A Config File!").AddText(error).Show();
//            }
//        }
//    }

//}
