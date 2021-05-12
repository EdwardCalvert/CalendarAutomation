using APIMethods;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinAPIBrightnessControl;

namespace GoogleCalender
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public BrightnessWorker _brightnessWorker;
        System.Windows.Forms.Timer clockTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer APITimer = new System.Windows.Forms.Timer();
        private Dictionary<string, List<string>> commandHistory = new Dictionary<string, List<string>> { };
        private int day = DateTime.Now.Day;
        private string id;

        private string GetId()
        {
            return id;
        }

        private void SetId(string value)
        {
            id = value;
        }

        Events events { get; set; }
        //

        //Added by google API

        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {
            _brightnessWorker = new BrightnessWorker();
            int second = DateTime.Now.Second;
            InitializeComponent();
            DisplayClock.Text = CurrentTime;

            CalibrateClockTimer();
            clockTimer.Tick += new EventHandler(this.ClockTimerCallback);



            GoogleAPI();
            //ClassroomAPI();
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



        private void GoogleAPI()
        {
            // List events.
            try
            {
                events = CalendarAPI.CallendarCallout(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "credentials.json"));
                UpdateTextBox();
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

        private bool StartedYet(DateTime dateTime)
        {
            if (DateTime.Compare(DateTime.Now, dateTime) <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool FinishedYet(DateTime dateTime)
        {
            if (DateTime.Compare(DateTime.Now, dateTime) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ReadCalendarItem(Event item)
        {
            var startTime = item.Start.DateTime;
            if (DebugMode)
            {
                InfoMessage(startTime.ToString(), "Clock.exe");
                DateTime.Compare(DateTime.Now, (DateTime)startTime);
            }

            string payload = item.Description;
            if (payload != null)
            {
                SetId(item.Id);
                string[] lines = payload.Split(new string[] { "<br>" }, StringSplitOptions.None);
                foreach (string line in lines)
                {

                    //InfoMessage("Normal line:"+line, "Clock.exe", DebugMode);

                    string cleanLine = ParseHTML(line);
                    InfoMessage("Stripped line :" + cleanLine, "Clock.exe", DebugMode);
                    if (cleanLine.StartsWith("["))
                    {
                        if (cleanLine.StartsWith("[delay)"))
                        {
                            InfoMessage("Delay method still not functional", "Clock.exe", DebugMode);


                        }
                        else if (cleanLine.StartsWith("[exe]"))
                        {

                            ExeHandler(cleanLine);
                        }
                    }
                    else
                    {

                        URLHandler(cleanLine);
                    }

                }

            }

        }

        private string ParseHTML(string inputHTML)
        {
            string noHTMLNormalised = System.Text.RegularExpressions.Regex.Replace(System.Text.RegularExpressions.Regex.Replace(inputHTML, @"<[^>]+>|&nbsp;", "").Trim(), @"\s{2,}", " ");
            return noHTMLNormalised;
        }


        private void ExeHandler(string line)
        {
            try
            {
                string program = @line.Substring(line.IndexOf("]") + 1);
                if (File.Exists(program) && !TabOpened(program))
                {

                    System.Diagnostics.Process.Start(program);
                    MarkTabAsOpen(program);
                    InfoMessage("Started process: " + program, "Clock.exe", DebugMode);
                }
                else
                {
                    if (File.Exists(ParseHTML(program)) && !TabOpened(program))
                    {
                        System.Diagnostics.Process.Start(ParseHTML(program));
                        MarkTabAsOpen(program);
                        InfoMessage("Started process: " + program, "Clock.exe", DebugMode);
                    }
                    else
                    {
                        if (!TabOpened(program))
                        {
                            WarningMessage(program + " does not exist.", "Can't find program");
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                WarningMessage("Fatal: " + exception.ToString(), "Can't find program");
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

        private void MarkTabAsOpen(string url)
        {
            if (DateTime.Now.Day == day)
            {
                if (!commandHistory.ContainsKey(GetId()))
                {
                    commandHistory[GetId()] = new List<string>();
                }
                commandHistory[GetId()].Add(url);


            }
            else
            {
                commandHistory = new Dictionary<string, List<string>> { };
            }
        }

        private bool TabOpened(string url)
        {

            if (DateTime.Now.Day == day && commandHistory.ContainsKey(GetId()) && commandHistory[GetId()].Contains(url))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void URLHandler(string line)
        {
            //C:\Program Files (x86)\Microsoft\Skype for Desktop\Skype.exe
            if (line.StartsWith("https://") || line.StartsWith("http://") || line.StartsWith("www."))
            {
                OpenUri(line);
            }
            if (line.StartsWith("<a href="))
            {
                List<string> urls = UnpackFrameURL(line);
                foreach (string url in urls)
                {

                    OpenUri(url);

                }
            }
        }

        private bool StartingNow(DateTime dateTime)
        {
            return DateTime.Compare(DateTime.Now, dateTime) == 0;
        }

        private void UpdateTextBox()
        {
            if (events.Items != null && events.Items.Count > 0)
            {
                UpdateEventText("", 'o');
                foreach (var eventItem in events.Items)
                {
                    if (StartedYet((DateTime)eventItem.Start.DateTime) && !FinishedYet((DateTime)eventItem.End.DateTime))
                    {
                        UpdateEventText(eventItem.Summary, 'a');
                        ReadCalendarItem(eventItem);
                    }

                }
            }
            else
            {
                NextUp.Text = "Calendar Empty";
            }
        }



        /// <param name="item"></param>
        /// <param name="mode"> Use 'a' for append or 'o' for override </param>
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

        public static bool IsValidUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }

        public static List<string> UnpackFrameURL(string frame)
        {

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(frame);
            var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            if (nodes == null)
            {
                return new List<string>();
            }
            else
            {
                return nodes.ToList().ConvertAll(r => r.Attributes.ToList().ConvertAll(i => i.Value)).SelectMany(j => j).ToList();
            }
        }

        public void OpenUri(string uri)
        {
            if (!TabOpened(uri) && IsValidUri(uri))
            {
                System.Diagnostics.Process.Start(uri);
                MarkTabAsOpen(uri);

                if (DebugMode)
                {
                    InfoMessage(string.Format("Opened URL: {1} With Id :{0}", GetId(), uri), "Clock.exe");
                }
            }
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
            //InfoMessage("Calender will update after : " + APITimer.Interval + " miliseconds","Clock.exe");
        }


    }
}
