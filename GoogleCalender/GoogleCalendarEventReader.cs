using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace GoogleCalendar
{
    /// <summary>
    /// A Class which encapsulates all the Google Calendar methods. Self updating. Use data from this class to keep everything up-to date. 
    /// </summary>
    public class GoogleCalendarEventReader
    {
        //*************** Google calendar API Stuff
        static string[] CalendarScope = { CalendarService.Scope.CalendarReadonly };
        static string CalendarAppName = "EDDEV101 Google Calendar API Service";
        private UserCredential _calendarReadCredential;
        private CalendarService _calendarReadService;
        //*******************************************

        //*********************Constants for Google calendar
        private string _nextPageToken;
        private int _pageSize;
        const int MAXRECURSIONDEPTH = 3; //Prevents the Get request from getting out of hand. Will fail with more than 15 all day events!
        private double _refreshTimeInMinutes;
        //***************************************

        //**Acutal variables
        private Events _lastCalloutEvents;
        private DateTime _lastRefreshTime;
        private Event _currentEvent;
        private List<URI> _uriList;
        private readonly System.Timers.Timer _autoRefreshTimer;
        private int _calloutsMade = 0;
       // private System.Timers.Timer _oauthRefreshTimer;
       //****************************************************

        public enum Result
        {
            Success,
        }

        private GoogleCalendarEventReader(int pagesize, double refreshTime)
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
            _refreshTimeInMinutes = refreshTime;

            //_autoRefreshTimer = new System.Timers.Timer(6000) { AutoReset = true };
            //_autoRefreshTimer.Elapsed += RefreshTimerEllapsed;
            //StartAutoRefresh();

            //_oauthRefreshTimer = new System.Timers.Timer(3596400); //one hour in milliseconds

            // _oauthRefreshTimer.Elapsed += new ElapsedEventHandler(RefreshOauthToken);
            // _oauthRefreshTimer.Start();

            _calendarReadService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _calendarReadCredential,
                ApplicationName = CalendarAppName,
            });
        }

        public GoogleCalendarEventReader() : this(5,1.0)
        {

        }

        /// <summary>
        /// The Oauth Token granted by Google Expires after 1 hour. This method refreshes the token, to ensure we can continue to access services. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void RefreshOauthToken(object source, ElapsedEventArgs e)
        {
            _calendarReadCredential.RefreshTokenAsync(CancellationToken.None);
        }

        //public void StopAutoRefresh()
        //{
        //    _autoRefreshTimer.Stop();
        //}

        //private void SyncTimer()
        //{
        //    _autoRefreshTimer.Interval = 60000 - (DateTime.Now.Second * 1000);

        //}

        //public void StartAutoRefresh()
        //{
        //    SyncTimer();
        //    _autoRefreshTimer.Start();
        //}

        //private void RefreshTimerEllapsed(object sender, ElapsedEventArgs e)
        //{
        //    RefreshLastCalloutEvents();
        //    SyncTimer();
        //}
        private void MakeAPICallout()
        {
            // Define parameters of request.
            //Only requesting vital information. 
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
            _calloutsMade++;
            UpdateVariables();
        }

        private void UpdateVariables()
        {
            foreach (Event eventItem in _lastCalloutEvents.Items)
            {
                if (HasTheEventStarted(eventItem.Start.DateTime, eventItem.End.DateTime))
                { 
                   SetCurrentEvent(eventItem);
                    break;
                }
            }

            SetCurrentEvent(null);
        }

        private void ParseURIs()
        {
            _uriList=URIParser.ReadText( GetCurrentEventDescription());
        }

        public void InvokeURIs()
        {
            foreach(URI u in _uriList)
            {
                u.LaunchURI();
            }
        }

        private void SetCurrentEvent(Event eventItem)
        {
            //_currentEvent != null && 
            if (_currentEvent != eventItem)
            {
                _currentEvent = eventItem;
                ParseURIs();
            }

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
            if (DataNeedsRefreshing()| _calloutsMade ==0)
            {
                _nextPageToken = "";

                MakeAPICallout();
                int requestDepth = 1;



                while (!OneTaskWasReturned() && requestDepth < MAXRECURSIONDEPTH)
                {
                    UpdateNextPageToken();
                    MakeAPICallout();
                    requestDepth++;
                }
                _lastRefreshTime = DateTime.Now;
            }

        }

        private bool DataNeedsRefreshing()
        {
            if (DateTime.Now.Subtract(_lastRefreshTime).TotalMinutes >= _refreshTimeInMinutes)
                return true;
            else if(_lastRefreshTime.Minute != DateTime.Now.Minute)
            {
                return true;
            }
            else
            {
                return false;
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


        private Event GetCurrentEvent()
        {
            RefreshLastCalloutEvents();
            return _currentEvent;
        }

        public string GetCurrentEventDescription()
        {
            if (GetCurrentEvent() != null)
                return _currentEvent.Description;
            return "";
        }
        public string GetCurrentEventSummary()
        {
            if(GetCurrentEvent() != null) 
                return _currentEvent.Summary;
            return "";
        }

        private bool HasTheEventStarted(DateTime? startTime, DateTime? endTime)
        {
            if (startTime != null && endTime != null)
            {
                if (DateTime.Compare(DateTime.Now, (DateTime)startTime) <= 0 &&DateTime.Compare(DateTime.Now, (DateTime)endTime) < 0)
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
