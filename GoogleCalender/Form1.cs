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
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace GoogleCalender
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        IWebDriver seleniumDriver;
        public Form1()
        {
            InitializeComponent();
            GoogleAPI();
            DisplayClock.Text = CurrentTime();
            timer.Interval = 60000 - ReturnMinuteUnit() * 1000;
            timer.Tick += new EventHandler(this.UpdateClock);
            timer.Start();
            
            seleniumDriver = new EdgeDriver();
            KillTab();

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

        private void KillTab()
        {
            //Rotate Tabs
            seleniumDriver.SwitchTo().Window(seleniumDriver.WindowHandles[0]);
            IJavaScriptExecutor jscript = seleniumDriver as IJavaScriptExecutor;
            jscript.ExecuteScript("alert('Focus')");
            seleniumDriver.SwitchTo().Alert().Accept();
        }

        private void ReadCalendarItem(Event item)
        {
            string payload = item.Description;
            if (payload != null)
            {
                //Do Some Processing here 
                string[] lines = payload.Split('\n');
                foreach(string line in lines)
                {
                    if (line.StartsWith("https://")|| line.StartsWith("http://"))
                    {
                        OpenUri(line);
                    }
                }
                
            }
        }

        private void UpdateTextBox(Events events)
        {
            if (events.Items != null && events.Items.Count > 0)
            {
                NextUp.Text = "";
                //label2.Text = "";
                foreach (var eventItem in events.Items)
                {
                    NextUp.Text += eventItem.Summary;
                    ReadCalendarItem(eventItem);
                }
            }
            else
            {
                NextUp.Text = "Nothing to do";
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

        public static bool OpenUri(string uri)
        {
            if (!IsValidUri(uri))
                return false;
            System.Diagnostics.Process.Start(uri);
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

    }
}
