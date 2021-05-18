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

namespace GoogleCalender
{
    public class TestClass
    {
        //*************** Google calendar API Stuff
        private static string[] CalendarScope = { CalendarService.Scope.Calendar };
        private static string CalendarAppName = "EDDEV101 Google Calendar API Event Liseter Service";
        private UserCredential _calendarReadCredential;
        private CalendarService _calendarReadService;
        //*******************************************


        public TestClass()
        {
            //Create new credeintial set.
            using (var stream =
               new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "writeToken.json";
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


            // Create the service.
            _calendarReadService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _calendarReadCredential,
                ApplicationName = "Google Calendar API Sample",
            });

        }

        public async void BatchRequest()
        {

            // Create a batch request.
            var request = new BatchRequest(_calendarReadService);
            request.Queue<CalendarList>(_calendarReadService.CalendarList.List(),
                 (content, error, i, message) =>
                 {
                     // Put your callback code here.
                 });
            request.Queue<Event>(_calendarReadService.Events.Insert(
                 new Event
                 {
                     Summary = "Learn how to execute a batch request",
                     Start = new EventDateTime() { DateTime = new DateTime(2021, 5, 18, 10, 0, 0) },
                     End = new EventDateTime() { DateTime = new DateTime(2021, 5, 18, 12, 0, 0) }
                 }, "primary"),
                 (content, error, i, message) =>
                 {
                     // Put your callback code here.
                 });
            // You can add more Queue calls here.

            // Execute the batch request, which includes the 2 requests above.
            await request.ExecuteAsync();
        }
        
    }
}
