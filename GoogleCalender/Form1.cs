using GoogleCalendar;
using System;
using System.Drawing;
using System.Windows.Forms;
using WinAPIBrightnessControl;

namespace GoogleCalendarWPF
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public BrightnessWorker _brightnessWorker;
        private GoogleCalendarEventReader _eventReader;
        private System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();
            SetWindowStartLocation();
            CalibrateClockTimer();
            
            clockTimer.Tick += new EventHandler(this.ClockTimerCallback);
            clockTimer.Start();

            

            _eventReader = new GoogleCalendarEventReader();
            UpdateUI();
        }

        private void SetWindowStartLocation()
        {
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Right - 170,
                          Screen.PrimaryScreen.Bounds.Height - 90);
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
            //_eventReader.InvokeURIs();
           // try
            //{
                UpdateEventText(_eventReader.GetCurrentEventSummary(), 'o');
           // }
            //catch (Exception exception)
            //{
                //NextUp.Text = "Error occurred, trying again." + exception + "Clock.exe";
               // CalibrateClockTimer();
           // }
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
    }
}