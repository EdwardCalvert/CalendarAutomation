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
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private Dictionary<string, List<string>> commandHistory= new Dictionary<string, List<string>> { };
        //Dictionary, where the time is the key ie "**:**", and string array represents visited URLS
        private int day = DateTime.Now.Day;
        string Id { get; set; }
        //

        //Added by google API
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";
        


        public Form1()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(this.UpdateClock);
            DisplayClock.Text = CurrentTime;
            int second = DateTime.Now.Second;
            timer.Interval = (60000 - (second * 1000));
            timer.Start();

        }
        

        private void GoogleAPI()
        {
            UserCredential credential;

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
            Events events = request.Execute();
            UpdateTextBox(events);
            

            
        }

        private int ReturnMinuteUnit()
        {
            int minute ;
            int currentMinute = DateTime.Now.Minute;
            if (currentMinute > 9)
            {
                minute = int.Parse(DateTime.Now.Minute.ToString().ToArray()[1].ToString());
            }
            else
            {
                minute = currentMinute;
            }
            return minute;
        }



        private void ReadCalendarItem(Event item)
        {
            
            string payload = item.Description;
            if (payload != null)
            {
                Id = item.Id;
                //Do Some Processing here 
                string[] lines = payload.Split(new string[] { "<br>" }, StringSplitOptions.None);
                foreach(string line in lines)
                {
                    //Possible casses
                    //Before - open no webpage
                    //During - ony open webpage if not already opend
                    //Dealayed execution- open webpage at desired time
                    //After? Do nothing. 

                    //Actions Open URL
                    //Open App <-Not started? Will I ever do this? 

                    //[delay;#########] where ####### represents a delay in minutes
                    //[delay_until;##:##] where ##:## represents a clock hour
                    //[delay_until_-#######] where -##### represents hours back from end time
                    //[exe] will start a windows process with this name, however, no validation is checked (i trust myself)

                    //KNOWN BUG: domain entered without protocol, won't be handeled by URLHandler. Not much I can do about that.
                    if (DebugMode)
                    {
                        InfoMessage("Normal line:"+line, "Clock.exe");
                    }
                    string cleanLine = ParseHTML(line);
                    if (DebugMode)
                    {
                        InfoMessage("Stripped line :"+cleanLine, "Clock.exe");
                    }
                    if (cleanLine.StartsWith("["))
                    {
                        if (cleanLine.StartsWith("[delay)"))
                        {
                            if (DebugMode)
                            {
                                InfoMessage("Delay method still not functional", "Clock.exe");
                            }

                        }
                        else if (cleanLine.StartsWith("[exe]"))
                        {
                            
                            ExeHandler(cleanLine);
                        }
                    }
                    else //If no wildcards are used, user is implying start once. 
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
                    if (DebugMode)
                    {
                        InfoMessage("Started process: " + program, "Clock.exe");
                    }
                }
                else
                {
                    if (File.Exists(ParseHTML(program)) && !TabOpened(program))
                    {
                        System.Diagnostics.Process.Start(ParseHTML(program));
                        MarkTabAsOpen(program);
                        if (DebugMode)
                        {
                            InfoMessage("Started process: " + program, "Clock.exe");
                        }
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
                //MessageBox.Show(exception.ToString());
                WarningMessage("Fatal: " + exception.ToString(), "Can't find program");
            }
        }

        private void InfoMessage(string message,string title)
        {
            MessageBox.Show(message, title,MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
        }
        private void WarningMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void MarkTabAsOpen(string url)
        {
            if (DateTime.Now.Day == day)
            {
                if (!commandHistory.ContainsKey(Id))
                {
                    commandHistory[Id] = new List<string>();
                }
                commandHistory[Id].Add(url);


            }
            else
            {
                commandHistory = new Dictionary<string, List<string>> { };
            }
        }

        private bool TabOpened(string url)
        {
 
            if (DateTime.Now.Day == day&& commandHistory.ContainsKey(Id)&& commandHistory[Id].Contains(url))
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


        private void UpdateTextBox(Events events)
        {
            if (events.Items != null && events.Items.Count > 0)
            {
                NextUp.Text = "";
                foreach (var eventItem in events.Items)
                {
                    UpdateEvent(eventItem);
                    ReadCalendarItem(eventItem);
                }
            }
            else
            {
                NextUp.Text = "Nothing to do";
            }
        }

        private void UpdateEvent(Event item)
        {
            NextUp.Text += item.Summary;
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
                    InfoMessage(string.Format("Opened URL: {1} With Id :{0}",Id,uri), "Clock.exe");
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

        private void UpdateClock(object sender, EventArgs e)
        {
            DisplayClock.Text = CurrentTime;
            timer.Interval = 60000;
            GoogleAPI();
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
    }
}
