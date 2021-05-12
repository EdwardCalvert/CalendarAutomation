using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Classroom.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
namespace APIMethods
{
    public class ClassroomAPI
    {

        static string[] ClassroomScopes = { ClassroomService.Scope.ClassroomCoursesReadonly };
        static string ClassroomAppName = "APIMethods.ClassroomAPI";


        public static ListCoursesResponse ListActiveClasses()
        {
            UserCredential credential;
            if (File.Exists("classroomCredentials.json"))
            {
                using (var stream = new FileStream("classroomCredentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        ClassroomScopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Classroom API service.
                var service = new ClassroomService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ClassroomAppName,

                });


                //Request a list of the active courses
                CoursesResource.ListRequest request = service.Courses.List();

                //Only list active courses
                request.CourseStates = CoursesResource.ListRequest.CourseStatesEnum.ACTIVE;

                ListCoursesResponse response = request.Execute();

                return response;
            }
            else
            {
                throw new FileNotFoundException();
            }

        }

        public static Course GetCourse(string id)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("classroomCredentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    ClassroomScopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Classroom API service.
            var service = new ClassroomService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ClassroomAppName,

            });

            //Request a list of the active courses
            CoursesResource.GetRequest request = service.Courses.Get(id);


            Course response = request.Execute();

            return response;
        }

        //public static void ClassroomAPICallout()
        //{
        //    UserCredential credential;

        //    using (var stream =
        //        new FileStream("classroomCredentials.json", FileMode.Open, FileAccess.Read))
        //    {
        //        // The file token.json stores the user's access and refresh tokens, and is created
        //        // automatically when the authorization flow completes for the first time.
        //        string credPath = "token.json";
        //        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //            GoogleClientSecrets.Load(stream).Secrets,
        //            ClassroomScopes,
        //            "user",
        //            CancellationToken.None,
        //            new FileDataStore(credPath, true)).Result;
        //        Console.WriteLine("Credential file saved to: " + credPath);
        //    }

        //    // Create Classroom API service.
        //    var service = new ClassroomService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = ClassroomAppName,
        //    });

        //    // Define request parameters.
        //    CoursesResource.ListRequest request = service.Courses.List();
        //    request.PageSize = 10;

        //    // List courses.
        //    ListCoursesResponse response = request.Execute();
        //    Console.WriteLine("Courses:");
        //    if (response.Courses != null && response.Courses.Count > 0)
        //    {
        //        //foreach (var course in response.Courses)
        //        //{
        //        //    //InfoMessage(("{0} ({1})", course.Name, course.Id).ToString(), "Clock.exe");
        //        //    //Console.WriteLine("{0} ({1})", course.Name, course.Id);
        //        //}
        //    }
        //    else
        //    {
        //        Console.WriteLine("No courses found.");
        //    }
        //    Console.Read();

        //}
    }
}
