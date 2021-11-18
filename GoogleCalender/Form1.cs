using Google.Apis.Calendar.v3.Data;
using GoogleCalendar;
using GoogleCalender;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ArduninoBrightnessController;
using System.Threading;

namespace GoogleCalendarWPF
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public ArduninoBrightnessWorker _brightnessWorker;
        private GoogleCalendarEventLister _eventReader;
        private System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        private MemoryManager<URI> memoryManager = new MemoryManager<URI>();
        private GoogleCalendarEventUpdater eventUpdater = new GoogleCalendarEventUpdater();
        private Event @event;
        private GoogleCalendar.Settings<SettingsStruct> _settings;
        GoogleCalendar.Settings<IntellisenseSettings> intellisenseSettings;


        private const string SETTINGSNAME = "settings.json";

        public Form1()
        {
            _settings = new GoogleCalendar.Settings<SettingsStruct>(SETTINGSNAME, Environment.CurrentDirectory + @"\");
            intellisenseSettings = new GoogleCalendar.Settings<IntellisenseSettings>("Intellisense.json", Environment.CurrentDirectory + @"\");
            InitializeComponent();
            SetWindowStartLocation();
            CalibrateClockTimer();
            _brightnessWorker = new ArduninoBrightnessWorker();
            _brightnessWorker.Start();

            clockTimer.Tick += new EventHandler(this.ClockTimerCallback);
            clockTimer.Start();

            _eventReader = new GoogleCalendarEventLister();
            UpdateUI();
            Notification.PlaySound(_settings.settings.ProgramStartSound);

            //https://github.com/noxad/windows-toast-notifications
            //Alarm alarm = new Alarm();
            //alarm.Start();
            //Notification.BootNoise();

        }




        private void SetWindowStartLocation()
        {
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Right - 150,
                          Screen.PrimaryScreen.Bounds.Height - 70);
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
        }

        private void CalibrateClockTimer()
        {
            clockTimer.Interval = 60000 - (DateTime.Now.Second * 1000);
        }

        public void UpdateEventText(string text, char mode)
        {
            if (mode == 'a')
            {
                NextUp.Text += text;
            }
            else if (mode == 'o')
            {
                NextUp.Text = text;
            }
        }

        private void InfoMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void WarningMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private string CurrentTime
        {
            get
            {
                int hh = DateTime.Now.Hour;
                int mm = DateTime.Now.Minute;

                string time = "";

                if (hh < 10)
                {
                    time += "0" + hh;
                }
                else
                {
                    time += hh;
                }
                time += ":";
                if (mm < 10)
                {
                    time += "0" + mm;
                }
                else
                {
                    time += mm;
                }
                return time;
            }
        }

        private void UpdateUI()
        {
            DisplayClock.Text = CurrentTime;
            CalibrateClockTimer();
            try
            {
                @event = _eventReader.ReturnSingleTask();
                UpdateEventText(GCExtensionMethods.GetCurrentEventSummary(@event), 'o');
                if (@event != null && @event.Summary != null && _settings.settings.LaunchURLs)
                {
                    if (_settings.settings.IntellisenseForEvents)
                    {
                        foreach (KeyValuePair<string,string> s in intellisenseSettings.settings.Values)
                        {
                            if (@event.Summary.Contains(s.Key))
                            {
                                @event.Description += s.Value;
                            }
                        }

                    }
                    List<URI> uris = GCExtensionMethods.GetCurrentEventURIs(@event);
                    if (uris != null && uris.Count > 0)
                    {
                        foreach (URI uRI in uris)
                        {
                            if (!memoryManager.TabOpened(uRI, @event.Id))
                            {
                                memoryManager.MarkTabAsOpen(uRI, @event.Id);
                                uRI.LaunchURI();
                                Notification.InputSound();
                            }
                        }
                    }
                    else
                    {
                        string url = @event.HtmlLink;
                        URI uRI = new URI(@event.HtmlLink);
                        if (!memoryManager.TabOpened(uRI, @event.Id))
                        {
                            memoryManager.MarkTabAsOpen(uRI, @event.Id);
                            uRI.LaunchURI();
                            Notification.InputSound();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                UpdateEventText("Error occurred:" + exception, 'o');
                CalibrateClockTimer();
            }
            //Sort GOOGLE STUFF HERE
        }

        private void ClockTimerCallback(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void NextUp_Click(object sender, EventArgs e)
        {
        }

        public bool DebugMode => debugMode.CheckState.ToString() == "Unchecked";

        private void debugMode_CheckedChanged(object sender, EventArgs e)
        {
            if (DebugMode)
            {
                InfoMessage("Debugging started", "Clock.exe");
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            InfoMessage("Calender will update after :   miliseconds", "Clock.exe");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //What if next tasks 
            Event nextTask = _eventReader.ReturnNextTask("[static]");
            if (nextTask != null)
            {
                if (nextTask.Start.DateTime != null)
                {
                    DateTime timeOfNextTask = nextTask.Start.DateTime.Value;
                    int minutesToMoveForward = -(int)timeOfNextTask.Subtract(DateTime.Now).TotalMinutes;

                    List<Event> events = _eventReader.ReturnAllDaysTasks(DateTime.Today.AddHours(24));

                        List<Event> newEventTimes = GCExtensionMethods.UpdateEventsForward(events, minutesToMoveForward, "[static]");

                    eventUpdater.UpdateEvents(newEventTimes);
                    UpdateEventText("Pulled forward for you. " + minutesToMoveForward, 'o');
                }
            }
            else
            {
                //eventUpdater.UpdateEvents(_eventReader.ReturnAllDaysTasks(
                //DateTime.Today.AddHours(24)), -10, "[static]","primary");
                UpdateEventText("Pulled forward for you.", 'o');
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Event nextTask = _eventReader.ReturnNextTask("[static]");
            if (nextTask != null)
            {
                if (nextTask.Start.DateTime != null)
                {
                    DateTime timeOfNextTask = nextTask.Start.DateTime.Value;
                    int minutesToMoveForward = 15;

                    List<Event> events = _eventReader.ReturnAllDaysTasks(DateTime.Today.AddHours(24));

                    List<Event> newEventTimes = GCExtensionMethods.UpdateEventsForward(events, minutesToMoveForward, "[static]");

                    eventUpdater.UpdateEvents(newEventTimes);
                    UpdateEventText("Pulled forward for you. " + minutesToMoveForward, 'o');
                }
            }
            else
            {
                //eventUpdater.UpdateEvents(_eventReader.ReturnAllDaysTasks(
                //DateTime.Today.AddHours(24)), -10, "[static]","primary");
                UpdateEventText("Pulled forward for you.", 'o');
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://calendar.google.com/calendar/u/0/r");
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {


                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipText = "Welcome to TutorialsPanel.com!!";
                notifyIcon1.BalloonTipTitle = "Welcome Message";
                notifyIcon1.ShowBalloonTip(2000);
            
        }
    }
}