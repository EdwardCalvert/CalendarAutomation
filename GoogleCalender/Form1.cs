using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace GoogleCalender
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private Dictionary<string, List<string>> tabMemory= new Dictionary<string, List<string>> { };
        //Dictionary, where the time is the key ie "**:**", and string array represents visited URLS
        private int day = DateTime.Now.Day;


        public Form1()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(this.UpdateClock);
            DisplayClock.Text = CurrentTime();
            int second = DateTime.Now.Second;
            timer.Interval = (60000 - (second * 1000));
            timer.Start();

        }
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

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
            string  id = item.Id;
            string payload = item.Description;
            if (payload != null)
            {
                //Do Some Processing here 
                string[] stringSeparators = new string[] { "<br>" };
                string[] lines = payload.Split(stringSeparators, StringSplitOptions.None);
                foreach(string line in lines)
                {
                    //Possible casses
                    //Before - open no webpage
                    //During - ony open webpage if not already opend
                    //Dealayed execution- open webpage at desired time
                    //After? Do nothing. 

                    //Actions Open URL
                    //Open App <-Not started? Will I ever do this? 

                    //Syntax
                    //[delay;#########] where ####### represents a delay in minutes
                    //[delay_until;##:##] where ##:## represents a clock hour
                    //[delay_until_-#######] where -##### represents hours back from end time
                    //[before_by;#####] where ##### represents minutes before. In practise, this will never be called, 
                    //[start] or nothing (implied) opens at start

                    //KNOWN BUG: domain entered without protocol, won't be handeled by URLHandler. Not much I can do about that.



                    if (line.StartsWith("["))
                    {
                        int endIndex = line.IndexOf(']');
                        int textEnd = line.IndexOf(';');
                        if (line.StartsWith("[delay_unitl;"))
                        {


                        }
                        else if (line.StartsWith("[delay_by;"))
                        { }
                        else if (line.StartsWith("[before_by;"))
                        { }
                        else if (line.StartsWith("[start]"))
                        { }
                    }
                    else //If no wildcards are used, user is implying start once. 
                    {
                        URLHandler(line,id);
                    }
                    
                }
                
            }
        }

        private void MarkTabAsOpen(string url,string id)
        {
            if (DateTime.Now.Day == day)
            {
                if (!tabMemory.ContainsKey(id))
                {
                    tabMemory[id] = new List<string>();
                }
                tabMemory[id].Add(url);


            }
            else
            {
                tabMemory = new Dictionary<string, List<string>> { };
            }
        }

        private bool TabOpened(string url,string id)
        {
            if (DateTime.Now.Day == day)
            {
                if (tabMemory.ContainsKey(id))
                {
                    if (tabMemory[id].Contains(url))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void URLHandler(string line, string id)
        {
            if (line.StartsWith("https://") || line.StartsWith("http://") || line.StartsWith("www."))
            {
                OpenUri(line);
            }
            if (line.StartsWith("<a href="))
            {
                List<string> urls = UnpackFrameURL(line);
                foreach (string url in urls)
                {
                    if (!TabOpened(url, id))
                    {
                        OpenUri(url);
                        MarkTabAsOpen(url,id);
                    }

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

        public bool OpenUri(string uri)
        {
            if (!IsValidUri(uri))
                    return false;

            System.Diagnostics.Process.Start(uri);
            //TabOpened(time);
            return true;

        }

        private string CurrentTime()
        {
            timer.Interval = 60000;
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

        private void UpdateClock(object sender, EventArgs e)
        {
            DisplayClock.Text = CurrentTime();
            GoogleAPI();
        }

        private void NextUp_Click(object sender, EventArgs e)
        {

        }
    }
}
