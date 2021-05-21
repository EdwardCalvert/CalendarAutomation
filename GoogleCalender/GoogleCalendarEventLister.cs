using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Timers;
using System.Collections.Generic;
using GoogleCalender;

namespace GoogleCalendar
{

    public class GoogleCalendarEventLister
    {
        //*************** Google calendar API Stuff
        private static string[] CalendarScope = { CalendarService.Scope.CalendarReadonly };
        private static string CalendarAppName = "EDDEV101 Google Calendar API Event Liseter Service";
        private UserCredential _calendarReadCredential;
        private CalendarService _calendarReadService;
        //*******************************************

        //*********************Constants for Google calendar
        private string _nextPageToken;
        private string _calendarId;
        private int _pageSize;
        private const int MAXRECURSIONDEPTH = 3; //Prevents the Get request from getting out of hand. Will fail with more than 15 all day events!
        private double _refreshTimeInMinutes;
        //***************************************

        //**Acutal variables
        private Events _lastCalloutEvents;
        private DateTime _lastRefreshTime;

        //****************************************************

        private GoogleCalendarEventLister(int pagesize, double refreshTime, string CalendarId)
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
            _calendarId = CalendarId;
            _calendarReadService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _calendarReadCredential,
                ApplicationName = CalendarAppName,
            });

        }

        public GoogleCalendarEventLister() : this(5, 1.0,"primary")
        {
        }

        private void MakeAPICallout()
        {
            // Define parameters of request.
            //Only requesting vital information.
            EventsResource.ListRequest request = _calendarReadService.Events.List(_calendarId);
            request.TimeMin = DateTime.Now;
            request.TimeMax = DateTime.Today.AddHours(24);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.PageToken = _nextPageToken;
            request.Fields = "items(summary,description,start,end,id),nextPageToken";
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

        public static bool HasTheEventStarted(DateTime? startTime, DateTime? endTime)
        {
            if (startTime != null && endTime != null)
            {
                if (DateTime.Compare(DateTime.Now, (DateTime)startTime) <= 0 && DateTime.Compare(DateTime.Now, (DateTime)endTime) < 0)
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

        public Event ReturnSingleTask()
        {
            _lastCalloutEvents = null; // Making assumption this will be null CLEAR.
            _nextPageToken = "";
            MakeAPICallout();
            UpdateNextPageToken();
            int requestDepth = 1;
            //The problem: on an empty calendar, itterating 3 times wont help. However, on empty data,
            while (!OneTaskWasReturned()
                && requestDepth < MAXRECURSIONDEPTH)//One was returned method prevents null.
            {
                MakeAPICallout();
                UpdateNextPageToken();
                requestDepth++;
            }
            _lastRefreshTime = DateTime.Now;
            if (DataWasNotNull())
            {
                foreach (Event eventItem in _lastCalloutEvents.Items)
                {
                    if (HasTheEventStarted(eventItem.Start.DateTime, eventItem.End.DateTime))
                    {
                        return eventItem;
                    }
                }
            }
            return null;
        }
        
        private Events RequestNMoreEvents(string pageToken, int number)
        {
            EventsResource.ListRequest request = _calendarReadService.Events.List(_calendarId);
            request.TimeMin = DateTime.Now;
            request.TimeMax = DateTime.Today.AddHours(24);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.PageToken = pageToken;
            request.Fields = "items(summary,description,start,end,id),nextPageToken";
            request.MaxResults = number;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            return request.Execute();
        }

        private static Event ReturnNextEvent(Events events)
        {
            foreach(Event e in events.Items)
            {
                if (e.Start.DateTime.HasValue && e.Start.DateTime.Value.CompareTo(DateTime.Now) >= 0)
                {
                    return e;
                }
            }
            return null;
        }

        private static Event ReturnNextEvent(Events events, string keyword)
        {
            foreach(Event e in events.Items)
            {
                if(e.Start.DateTime.HasValue && e.Start.DateTime.Value.CompareTo(DateTime.Now) >= 0)
                {
                    if(e.Description == null)
                    {
                        return e;
                    }
                    else if (!e.Description.Contains(keyword))
                    {
                        return e;
                    }
                }
            }
            return null;
        }


        public Event ReturnNextTask(string keyword)
        {
            Events events = RequestNMoreEvents("", 15);//Limiting to a search of 15 events. Otherwise could get crazy.
            if (HowManyTasks(events) >= 2)
            {
                return ReturnNextEvent(events, keyword);
            }
            //Intentionally returning null to indicate that ther is no second task. 
            return null;
        }

        public List<Event> ReturnAllDaysTasks(DateTime endOfWorkingDay)
        {
            EventsResource.ListRequest request = _calendarReadService.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.TimeMax = endOfWorkingDay;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = GetMinutesUntil(endOfWorkingDay)/5 ;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            Events events = request.Execute();
             return GCExtensionMethods.OnlyShortEventsToList(events);
        }

        private int GetMinutesUntil(DateTime time)
        {
            return (int) time.Subtract(DateTime.Now).TotalMinutes;
        }
        private int GetMinutesUntil()
        {
            return (int)DateTime.Today.AddHours(24).Subtract(DateTime.Now).TotalMinutes;
        }

        private bool DataNeedsRefreshing()
        {
            return (DateTime.Now.Subtract(_lastRefreshTime).TotalMinutes >= _refreshTimeInMinutes);
        }

        private bool DataWasNotNull()
        {
            return _lastCalloutEvents.Items != null && _lastCalloutEvents.Items.Count > 0;
        }

        private bool OneTaskWasReturned()
        {
            if (DataWasNotNull())
            {
                foreach (Event EventItem in _lastCalloutEvents.Items)
                {
                    if (EventItem.Start.DateTime != null)
                    {
                        //This condition is only met if the event has a start time, hence, can't be all day event!
                        return true;
                    }
                }
            }
            return false;
        }

        private int HowManyTasks(Events events)
        {
            int count = 0;
            if (DataWasNotNull())
            {
                foreach (Event EventItem in events.Items)
                {
                    if (EventItem.Start.DateTime != null)
                    {
                        //This condition is only met if the event has a start time, hence, can't be all day event!
                        count++;
                    }
                }
            }
            return count;
        }


    }
}