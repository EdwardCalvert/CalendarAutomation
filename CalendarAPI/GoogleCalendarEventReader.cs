using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Timers;

namespace GoogleCalendarAPIs
{
    /// <summary>
    /// A Class which encapsulates all the Google Calendar methods. Self updating. Use data from this class to keep everything up-to date. 
    /// </summary>
    public class GoogleCalendarEventReader
    {
        static string[] CalendarScope = { CalendarService.Scope.CalendarReadonly };
        static string CalendarAppName = "EDDEV101 Google Calendar API Service";

        //Events events;
        private Events _lastCalloutEvents;
        private string _nextPageToken;
        private int _pageSize;
        private System.Timers.Timer _oauthRefreshTimer;
        private UserCredential _calendarReadCredential;
        private CalendarService _calendarReadService;

        public GoogleCalendarEventReader(int pagesize)
        {
            //Create new credeintial set.
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

            _pageSize = pagesize;

            _oauthRefreshTimer = new System.Timers.Timer(60 * 60 * 999); //one hour in milliseconds

            _oauthRefreshTimer.Elapsed += new ElapsedEventHandler(RefreshOauthToken);
            _oauthRefreshTimer.Start();

            _calendarReadService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _calendarReadCredential,
                ApplicationName = CalendarAppName,
            });
        }

        public GoogleCalendarEventReader() : this(5)
        {

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

        public Events GetCurrentEvents()
        {
            RefreshLastCalloutEvents();
            return _lastCalloutEvents;
        }

        private  void MakeAPICallout()
        {
            // Define parameters of request.
            EventsResource.ListRequest request = _calendarReadService.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.TimeMax = DateTime.Today.AddHours(24);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.PageToken = _nextPageToken;
            request.Fields = "items(summary,description,start,end),nextPageToken";
            request.MaxResults = _pageSize;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            _lastCalloutEvents = request.Execute();
        }

        private void UpdateNextPageToken()
        {
            if (_lastCalloutEvents.NextPageToken != null)
            {
                _nextPageToken = _lastCalloutEvents.NextPageToken;
            }
        }
        //It would be bonkers to ask for all events from Google!
        //So we'll batch it until we find the current task!

        private void RefreshLastCalloutEvents()
        {
            _nextPageToken = "";
            MakeAPICallout();

            // Define parameters of request.
            //Only requesting vital information. 

            while (!OneTaskWasReturned())
            {
                UpdateNextPageToken();
                MakeAPICallout();
            }

        }


        private bool OneTaskWasReturned()
        {
            foreach(Event EventItem in _lastCalloutEvents.Items)
            {
                if(EventItem.Start.DateTime != null) 
                {
                    //This condition is only met if the event has a start time, hence, can't be all day event!
                    return true;
                }
            }
            return false;
        }

        private bool StartedYet(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                if (DateTime.Compare(DateTime.Now, (DateTime)dateTime) <= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private bool FinishedYet(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                if (DateTime.Compare(DateTime.Now, (DateTime)dateTime) < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
