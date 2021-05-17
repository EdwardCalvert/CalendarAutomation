using Google.Apis.Calendar.v3.Data;
using GoogleCalendar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinAPIBrightnessControl;


namespace GoogleCalendarWPF
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public BrightnessWorker _brightnessWorker;
        private GoogleCalendarEventReader _eventReader;
        System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer APITimer = new System.Windows.Forms.Timer();
        //private Dictionary<string, List<string>> commandHistory = new Dictionary<string, List<string>> { };
        //private int day = DateTime.Now.Day;
        //private string id;

        //private string GetId()
        //{
        //    return id;
        //}

        //private void SetId(string value)
        //{
        //    id = value;
        //}

        //

        //Added by google API

        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {

            //_brightnessWorker = new BrightnessWorker();
            //_brightnessWorker.Start();
            int second = DateTime.Now.Second;
            InitializeComponent();
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Right-170, //should be (0,0)
                          Screen.PrimaryScreen.Bounds.Height-90);
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;

            DisplayClock.Text = CurrentTime;

            CalibrateClockTimer();
            clockTimer.Tick += new EventHandler(this.ClockTimerCallback);

            _eventReader = new GoogleCalendarEventReader();
            
            CalibrateAPITimer();
            APITimer.Tick += new EventHandler(this.APITimerCallback);
            APITimer.Start();
            clockTimer.Start();

        }

        private void CalibrateAPITimer()
        {
            APITimer.Interval = (60000 - (DateTime.Now.Second * 1000)) + 1000;
        }

        private void CalibrateClockTimer()
        {
            clockTimer.Interval = (60000 - (DateTime.Now.Second * 1000));
            //)+1000
        }

        private void ProcessEventAndUpdate(Event @event)
        {
            UpdateEventText("", 'o');
            URIParser.ReadText(@"https://cheese.com");
        }

        private void GoogleAPI()
        {
            // List events.
            try
            {
                ProcessEventAndUpdate(_eventReader.GetCurrentEvent());
                //events = APIMethods.CalendarAPI.CallendarCallout(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "credentials.json"));
               // UpdateTextBox();
            }
            catch (Exception exception)
            {
                if (DebugMode)
                {
                    WarningMessage("Request could not be fuffiled: " + exception, "Clock.exe");
                }
                else
                {
                    NextUp.Text = "Error occurred, trying again." + exception + "Clock.exe";
                }
                CalibrateAPITimer();
                CalibrateClockTimer();
            }

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
        private void InfoMessage(string message, string title, bool show)
        {
            if (show)
            {
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
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

        private void ClockTimerCallback(object sender, EventArgs e)
        {
            DisplayClock.Text = CurrentTime;
            CalibrateClockTimer();
        }

        private void APITimerCallback(object sender, EventArgs e)
        {

            GoogleAPI();
            CalibrateAPITimer();
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
            GoogleAPI();
            InfoMessage("Calender will update after : " + APITimer.Interval + " miliseconds","Clock.exe");
        }


    }
}
