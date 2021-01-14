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
        public Form1()
        {
            
            InitializeComponent();
            GoogleAPI();
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
            if (events.Items != null && events.Items.Count > 0)
            {
                NextUp.Text = "";
                //label2.Text = "";
                foreach (var eventItem in events.Items)
                {
                    NextUp.Text += eventItem.Summary + Environment.NewLine;
                    // NextUp.Text += eventItem.Description+Environment.NewLine;
                }
            }
            else
            {
                NextUp.Text = "Nothing to do";
            }

            int minute =0;
            int currentMinute = DateTime.Now.Minute;
            if (currentMinute > 9)
                {
                 minute = int.Parse(DateTime.Now.Minute.ToString().ToArray()[1].ToString());
            }
            else
            {
                 minute = currentMinute;
            }
            DisplayClock.Text = CurrentTime();
            timer.Interval = 60000- minute*1000;
            timer.Tick += new EventHandler(this.UpdateClock);

            timer.Start();
            
        }

        private void UpdateTextBox()
        { 
            
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


        private void GetEvents_Tick(object sender, EventArgs e)
        {
            GoogleAPI();
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
