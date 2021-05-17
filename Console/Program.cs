using System;
using GoogleCalendarAPIs;
using Google.Apis.Calendar.v3.Data;

namespace ConsoleMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            GoogleCalendarEventReader eventReader = new();

            Events events = eventReader.GetCurrentEvents();
            foreach (Event EventItem in events.Items)
            {
                Console.WriteLine(EventItem.Summary);
            }
        }
    }
}
