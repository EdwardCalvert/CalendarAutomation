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

namespace GoogleCalender
{
    public class Rescheduler
    {
        //*************** Google calendar API Stuff
        private static string[] CalendarScope = { CalendarService.Scope.CalendarEvents };
        private static string CalendarAppName = "EDDEV101 Google Calendar API Event Rescheduler Service";
        private UserCredential _calendarReadCredential;
        private CalendarService _calendarReadService;
        //*******************************************



        public Rescheduler()
        {
            //Create new credeintial set.
            using (var stream =
               new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "ReschedulerCredentials.json";
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
        public void ResechuleEvents(int minutesToAdjust)
        {

        }


    }
}
