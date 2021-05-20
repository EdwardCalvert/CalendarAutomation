using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3.Data;
using GoogleCalendar;

namespace GoogleCalender
{
    public static class GoogleCalendarExtensionMethods
    {
        public static string GetCurrentEventDescription(Event e)
        {
            if (e != null && e.Description !=null)
                return e.Description;
            return "";
        }

        public static string GetCurrentEventSummary(Event e)
        {
            if (e != null && e.Summary != null)
                return e.Summary;
            return "";
        }
        public static List<URI> GetCurrentEventURIs(Event e)
        {
            return URIParser.ReadText(GetCurrentEventDescription(e));
        }

        public static List<Event> returnStaticEvents(Events events, string keyword)
        {
            List<Event> eventsList = new List<Event>();
            if (DataWasNotNull(events))
            {
                foreach(Event e in events.Items)
                {
                    if (e != null
                        && e.Start.DateTime != null
                        && e.Description != null 
                        && !e.Description.Contains(keyword))
                    {
                        eventsList.Add(e);
                    }
                }
            }
            return eventsList;
        }

        public static bool DataWasNotNull(Events events)
        {
            return events.Items != null && events.Items.Count > 0;
        }

        /// <summary>
        /// Returns true if event1 overlaps event 2i.e, event2 was there first.
        /// </summary>
        /// <param name="event1"></param>
        /// <param name="event2"></param>
        /// <returns></returns>
        public static bool DistinctEvents(Event event1, Event event2)
        {
            //https://jamboard.google.com/d/1i098r1WhMyf1cpYM_t-18cJmQvJQ_d-JDs0T8Gy7fLs/viewer?f=0
            return event1 != null &&
                event2 != null &&
                event1.Start.DateTime.HasValue &&
                event2.End.DateTime.HasValue &&
                (event1.End.DateTime.Value.CompareTo(event2.Start.DateTime.Value) <= -1 || 
                event2.Start.DateTime.Value.CompareTo(event1.End.DateTime.Value) >= -1);

        }
    }
}
