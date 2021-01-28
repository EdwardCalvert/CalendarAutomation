using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace GoogleCalender
{
    public partial class Form1 : Form
    {
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
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";



        public Form1()
        {
            int second = DateTime.Now.Second;
            InitializeComponent();
            DisplayClock.Text = CurrentTime;

            CalibrateClockTimer();
            clockTimer.Tick += new EventHandler(this.ClockTimerCallback);



            GoogleAPI();
            CalibrateAPITimer();
            APITimer.Tick += new EventHandler(this.APITimerCallback);
            APITimer.Start();
            clockTimer.Start();

        }

        private void CalibrateAPITimer()
        {
            APITimer.Interval = (60000 - (DateTime.Now.Second * 1000));
        }

        private void CalibrateClockTimer()
        {
            clockTimer.Interval = (60000 - (DateTime.Now.Second * 1000));
            //)+1000
        }

        private void GoogleAPI()
        {

            UserCredential credential;
            System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            var path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "credentials.json";
            InfoMessage(File.Exists(path).ToString(), "Clock.exe", DebugMode);
            if (true)//File.Exists(path))
            {
                using (var stream =
                    new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }


                // Create Google Calendar API service.
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Define parameters of request.
                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 1;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;


                // List events.
                try
                {
                    events = request.Execute();
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
                        NextUp.Text = "Error occurred, trying again.";
                    }
                    CalibrateAPITimer();
                    CalibrateClockTimer();
                }
            }
            else
            {
                UpdateEventText("Error: credentials.json doesn't exist.", 'o');
                DisplayClock.Text = "00:00";
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
                foreach(string line in lines)
                {

                    //InfoMessage("Normal line:"+line, "Clock.exe", DebugMode);

                    string cleanLine = ParseHTML(line);
                    InfoMessage("Stripped line :"+cleanLine, "Clock.exe", DebugMode);
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
            string noHTML = Regex.Replace(inputHTML, @"<[^>]+>|&nbsp;", "").Trim();
            string noHTMLNormalised = System.Text.RegularExpressions.Regex.Replace(noHTML, @"\s{2,}", " ");
            return noHTMLNormalised;
        }

        private void DelayExecution(string line)
        { 
            
        }

        private void ExeHandler(string line)
        {
            try
            {
                string program = @line.Substring(line.IndexOf("]") + 1);
                if (File.Exists(program)&&!TabOpened(program))
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

        private void InfoMessage(string message,string title)
        {
            MessageBox.Show(message, title,MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
        }
        private void InfoMessage(string message, string title,bool show)
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
 
            if (DateTime.Now.Day == day&& commandHistory.ContainsKey(GetId()) && commandHistory[GetId()].Contains(url))
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


        private void UpdateTextBox()
        {
            if (events.Items != null && events.Items.Count > 0)
            {
                UpdateEventText("", 'o');
                foreach (var eventItem in events.Items)
                {
                   //// InfoMessage(eventItem.Kind.ToString(), "Clcok.exe", DebugMode);
                   // InfoMessage((eventItem.Start.DateTime != null).ToString() +"Start Time Null", "Clock.exe", DebugMode);
                   // InfoMessage((eventItem.End.DateTime != null).ToString() + "End Time Null", "Clock.exe", DebugMode);

                   // DateTime startTime;
                   // if(DateTime.TryParse(eventItem.Start.DateTime, out startTime))
                   // {
                        

                   // }

                   // InfoMessage("Starte yet" + StartedYet((DateTime.TryParse())eventItem.Start.DateTime).ToString(),"Clock.ex",DebugMode);
                    //InfoMessage(eventItem.Summary.ToString(), "clock.exe", DebugMode);
                    //InfoMessage("Started yet: " + StartedYet((DateTime)eventItem.Start.DateTime) + " Finished yet: " + FinishedYet((DateTime)eventItem.End.DateTime) , "Clock.exe", DebugMode);
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
        private void UpdateEventText(string text,char mode)
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
                return  nodes.ToList().ConvertAll(r => r.Attributes.ToList().ConvertAll(i => i.Value)).SelectMany(j => j).ToList();
            }
        }

        public void OpenUri(string uri)
        {
            if (!TabOpened(uri)&&IsValidUri(uri))
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

        private bool DebugMode => debugMode.CheckState.ToString() == "Unchecked";

        private void debugMode_CheckedChanged(object sender, EventArgs e)
        {
            if (DebugMode)
            {
                InfoMessage("Debugging started","Clock.exe");
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            InfoMessage("Calender will update after : " + APITimer.Interval + " miliseconds","Clock.exe");
        }

        //Archived methods *******************************************************************************************************************
        //private int ReturnMinuteUnit()
        //{
        //    int minute;
        //    int currentMinute = DateTime.Now.Minute;
        //    if (currentMinute > 9)
        //    {
        //        minute = int.Parse(DateTime.Now.Minute.ToString().ToArray()[1].ToString());
        //    }
        //    else
        //    {
        //        minute = currentMinute;
        //    }
        //    return minute;
        //}
    }
}
