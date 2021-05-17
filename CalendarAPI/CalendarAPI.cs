using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;


namespace APIMethods
{
    public class CalendarAPI
    {
        //Events events { get; set; }
        //

        //Added by google API
        static string[] CalendarScope = { CalendarService.Scope.CalendarReadonly };
        static string CalendarAppName = "Callendar Clock Callout";


        public static Events CallendarCallout(string credentialsFilePath)
        {
            UserCredential credential;
            if (File.Exists(credentialsFilePath))
            {
                using (var stream =
                    new FileStream(credentialsFilePath, FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        CalendarScope,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }


                // Create Google Calendar API service.
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = CalendarAppName,
                });

                // Define parameters of request.
                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 1;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                return request.Execute();
            }
            else
            {
                throw new FileNotFoundException();
            }
        }



    }

}