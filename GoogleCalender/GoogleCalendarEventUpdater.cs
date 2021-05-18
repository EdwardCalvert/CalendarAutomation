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
using Google.Apis.Requests;
using System.Diagnostics;

namespace GoogleCalender
{
    public class GoogleCalendarEventUpdater
    {
        //*************** Google calendar API Stuff
        private static string[] CalendarScope = { CalendarService.Scope.CalendarEvents };
        private static string CalendarAppName = "EDDEV101 Google Calendar API Event Liseter Service";
        private UserCredential _calendarReadCredential;
        private CalendarService _calendarReadService;
        //*******************************************



        public  GoogleCalendarEventUpdater()
        {
            //Create new credeintial set.
            using (var stream =
               new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "writingCredentials.json";
                _calendarReadCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    CalendarScope,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            _calendarReadService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _calendarReadCredential,
                ApplicationName = CalendarAppName,
            });


        }

        //   public void UpdateEvents(Events events)
        //   {


        //       if (GoogleCalendarExtensionMethods.DataWasNotNull(events))
        //       {
        //           var request = new BatchRequest(service);
        //           request.Queue<CalendarList>(service.CalendarList.List(),
        //                (content, error, i, message) =>
        //                {
        //    // Put your callback code here.
        //});
        //           BatchRequest();
        //       }
        //   }
        private void BringEventForward( Event e, string CalendarID)
        {
            var request = _calendarReadService.Events.Update(e, CalendarID, e.Id);
            request.Execute();
        }

        private Event UpdateStartTime(Event e,int minutesMoveFowardard)
        {
            e.Start.DateTime = e.Start.DateTime.Value.AddMinutes(minutesMoveFowardard);
            e.End.DateTime = e.End.DateTime.Value.AddMinutes(minutesMoveFowardard);
            return e;
        }

        private bool NewTimeConflictsWithStaticEvents(List<Event> staticEvents, Event e)
        {
            if(staticEvents != null)
            {
                foreach(Event event1 in staticEvents)
                {
                    if (GoogleCalendarExtensionMethods.DistinctEvents(event1, e)) ;
                    {

                    }
                }
            }
            return false;
        }

        public void UpdateEvents(Events events, 
            List<Event> staticEvents,
            int minutesMoveFowardard,
            string keyword, 
            string CalendarID)
        {
            if (GoogleCalendarExtensionMethods.DataWasNotNull(events))
            {
                foreach(Event e in events.Items)
                {
                    if (e.Start.DateTime != null )
                    {
                        if (e.Description == null)
                        {

                            Event newevent = UpdateStartTime(e, minutesMoveFowardard);
                            BringEventForward( e, CalendarID);
                        }
                        else if (e.Description !=null && !e.Description.Contains(keyword))
                        {
                            BringEventForward( e, CalendarID);
                        }
                    }
                }

            }
        }


        //public void UpdateEvent(Event e, int minutesMoveFowardard, string keyword, string CalendarID)
        //{
        //    if (e != null)
        //    {
        //        if (e.Start.DateTime != null)
        //        {
        //            if (e.Description == null)
        //            {
        //                BringEventForward(minutesMoveFowardard, e, CalendarID);
        //            }
        //            else if (e.Description != null && !e.Description.Contains(keyword))
        //            {
        //                BringEventForward(minutesMoveFowardard, e, CalendarID);
        //            }
        //        }

        //    }
        //}

    }
}
