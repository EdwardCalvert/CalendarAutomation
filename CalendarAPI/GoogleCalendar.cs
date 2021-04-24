using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Timers;

namespace APIMethods
{
    /// <summary>
    /// A Class which encapsulates all the Google Calendar methods. Self updating. Use data from this class to keep everything up-to date. 
    /// </summary>
    class GoogleCalendar
    {
        static string[] CalendarScope = { CalendarService.Scope.CalendarReadonly };
        static string CalendarAppName = "Callendar Clock Callout";

        Events events;

        private System.Timers.Timer _timer;



        private UserCredential _calendarReadCredential;
        private CalendarService _calendarReadService;

        private GoogleCalendar()
        {
            using (var stream =
               new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                _calendarReadCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    CalendarScope,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            _timer = new System.Timers.Timer(60 * 60 * 999); //one hour in milliseconds

            _timer.Elapsed += new ElapsedEventHandler(RefreshOauthToken);
            _timer.Start();

            _calendarReadService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _calendarReadCredential,
                ApplicationName = CalendarAppName,
            });
        }

        /// <summary>
        /// The Oauth Token granted by Google Expires after 1 hour. This method refreshes the token, to ensure we can continue to access services. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void RefreshOauthToken(object source, ElapsedEventArgs e)
        {
            //THe refresh Token method returns true if it was sucessful. What to do if not. ???
            _calendarReadCredential.RefreshTokenAsync(CancellationToken.None);
        }

        public void GetEvents()
        {

        }


        public Events GetCurrentEvent
        {
            get
            {



                // Define parameters of request.
                EventsResource.ListRequest request = _calendarReadService.Events.List("primary");
                request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 1;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                return request.Execute();
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
    }
}
