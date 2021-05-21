using Google.Apis.Calendar.v3.Data;
using GoogleCalendar;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GoogleCalender
{
    public static class GCExtensionMethods
    {
        public static string GetCurrentEventDescription(Event e)
        {
            if (e != null && e.Description != null)
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

        public static (List<Event>, List<Event>) returnStaticEvents(List<Event> events, string keyword)
        {
            //var linqStaticList = from event1 in events where event1.Start.DateTime.HasValue && event1.Description != null && event1.Description.Contains(keyword) select event1;

            List<Event> staticList = new List<Event>();
            List<Event> moveList = new List<Event>();
            if (events != null)
            {
                foreach (Event e in events)
                {
                    if (e != null
                        && e.Start.DateTime != null
                        )
                    {
                        if (e.Description != null
                        && e.Description.Contains(keyword))
                            staticList.Add(e);
                        else
                            moveList.Add(e);
                    }
                }
            }
            return (staticList, moveList);
        }

        public static bool DataWasNotNull(Events events)
        {
            return events.Items != null && events.Items.Count > 0;
        }
        public static bool DataWasNotNull(Event events)
        {
            return events.Summary != null;
        }

        /// <summary>
        /// Returns true if event1 overlaps event 2i.e, event2 was there first.
        /// </summary>
        /// <param name="event1"></param>
        /// <param name="event2"></param>
        /// <returns></returns>
        public static bool DistinctEvents(Event mEvent, Event sEvent)
        {
            //https://jamboard.google.com/d/1i098r1WhMyf1cpYM_t-18cJmQvJQ_d-JDs0T8Gy7fLs/viewer?f=0
            return NullAndSingleEventCheck(mEvent, sEvent) &&
                (MEventBeforeSEvent(mEvent, sEvent) &&
                MEventAfterSEvent(mEvent, sEvent));
        }

        public static bool NullAndSingleEventCheck(Event mEvent, Event sEvent)
        {
            return mEvent != null &&
                sEvent != null &&
                mEvent.Start.DateTime.HasValue &&
                sEvent.End.DateTime.HasValue;
        }

        public static bool MEventBeforeSEvent(Event mEvent, Event sEvent)
        {

            bool result = mEvent.End.DateTime.Value.CompareTo(sEvent.Start.DateTime.Value) <= -1;
            //Debug.Write($" End @ {mEvent.End.DateTime.Value.TimeOfDay} vs start @ {sEvent.Start.DateTime.Value.TimeOfDay} Is End before start: {result}");
            return result;
        }

        public static bool MEventAfterSEvent(Event mEvent, Event sEvent)
        {

            bool result = mEvent.Start.DateTime.Value.CompareTo(sEvent.End.DateTime.Value) <= -1;
            // Debug.Write($" Start @ {mEvent.Start.DateTime.Value.TimeOfDay} vs end @ {sEvent.End.DateTime.Value.TimeOfDay} Is start before end: {result}");
            return result;
        }

        public static Dictionary<Event, List<Event>> Collsions(List<Event> moveableEvents, List<Event> staticEvents)
        {
            Dictionary<Event, List<Event>> collsions = new Dictionary<Event, List<Event>>();
            //int count2 = events1.Count;
            //List<Event> mEventColisions = new List<Event>();
            //List<Event> sEventCollsions = new List<Event>();
            //var sortedResults = from event1 in moveableEvents orderby event1.Start


            //staticEvents null? why bother
            //Loop through all events from both 
            if (staticEvents != null && staticEvents.Count > 0)
            {
                foreach (Event mEvent in moveableEvents)
                {
                    foreach (Event sEvent in staticEvents)
                    {
                        Debug.WriteLine($"Now comparing {mEvent.Summary} @ {mEvent.Start.DateTime.Value.TimeOfDay} - {mEvent.End.DateTime.Value.TimeOfDay} with {sEvent.Summary} @ {sEvent.Start.DateTime.Value.TimeOfDay} - {sEvent.End.DateTime.Value.TimeOfDay}");
                        if (!DistinctEvents(mEvent, sEvent))
                        {
                            Debug.WriteLine("Result was true");
                            if (!collsions.ContainsKey(sEvent))
                            {
                                collsions.Add(sEvent, new List<Event>() { sEvent });
                            }
                            else
                            {
                                collsions[sEvent].Add(sEvent);
                            }
                            //mEventColisions.Add(mEvent);
                            //sEventCollsions.Add(sEvent);
                        }
                        else
                        {
                            Debug.WriteLine("Result was false");
                        }
                    }
                }
            }
            return collsions;
        }



        private static bool StartedYet(DateTime dateTime)
        {
            if (DateTime.Compare(DateTime.Now, dateTime) <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private static void ResolveEventCollisions(List<Event> mEventCollsions, List<Event> sEventCollisions)
        {
            foreach (Event mEvent in mEventCollsions)
            {
                foreach (Event sEvent in sEventCollisions)
                {
                }
            }
        }

        private static bool FinishedYet(DateTime dateTime)
        {
            if (DateTime.Compare(DateTime.Now, dateTime) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static List<Event> UpdateEventsForward(List<Event> events, int offset)
        {
            //SO new tactic, when updating events chronoglogically, check if there is sufficent time when event is being moved. If there isn't Then leave it there?
            foreach (Event e in events)
            {
                if (StartedYet(e.Start.DateTime.Value))
                    e.End.DateTime = DateTime.Now;
                else
                {
                    e.Start.DateTime = e.Start.DateTime.Value.AddMinutes(offset);
                    e.End.DateTime = e.End.DateTime.Value.AddMinutes(offset);
                }
            }
            return events;
        }
        public static List<Event> UpdateEventsForward(List<Event> allEvents, int offset, string keyword)
        {
            //SO new tactic, when updating events chronoglogically, check if there is sufficent time when event is being moved. If there isn't Then leave it there?
            for (int i = 0; i < allEvents.Count; i++)
            {
                Event e = allEvents[i];
                //If description is null, assumed non static. 
                if (e != null && e.Start.DateTime != null && IsTask(e, keyword))//Non-static Event.
                {
                    if ( offset <0 && StartedYet(e.Start.DateTime.Value  )) // Fires if the current event has started.
                        e.End.DateTime = DateTime.Now;
                    else if (offset > 0 && StartedYet(e.Start.DateTime.Value))
                        e.End.DateTime = e.End.DateTime.Value.AddMinutes(offset);
                    else
                    {
                        if (i == 0) //Assume it will fit?
                        {
                            e.Start.DateTime = e.Start.DateTime.Value.AddMinutes(offset);
                            e.End.DateTime = e.End.DateTime.Value.AddMinutes(offset);
                        }
                        if (offset < 0) //Indicates a move forward - shortening time.
                        {

                            if (i > 0 && GapBetween(allEvents[i - 1], e) > EventLength(e))//Space to put event.
                            {
                                e.Start.DateTime = e.Start.DateTime.Value.AddMinutes(offset);
                                e.End.DateTime = e.End.DateTime.Value.AddMinutes(offset);
                            }
                            else //Gap between event is not long enough. hm...
                            {
                                //Debug.Write("Hete");
                            }
                        }
                        else
                        {

                            if(i == allEvents.Count)//Assume it will fit - unable to check?
                            {
                                e.Start.DateTime = e.Start.DateTime.Value.AddMinutes(offset);
                                e.End.DateTime = e.End.DateTime.Value.AddMinutes(offset);
                            }
                            if (i > 0 && allEvents[i+1].Start.DateTime.Value.Subtract(allEvents[i-1].End.DateTime.Value).TotalMinutes > EventLength(e))//Not Firing.
                            {
                                e.Start.DateTime = e.Start.DateTime.Value.AddMinutes(offset);
                                e.End.DateTime = e.End.DateTime.Value.AddMinutes(offset);
                            }
                        }
                    }
                }
            }
            return allEvents;
        }

        public static bool IsTask(Event e, string keyword)
        {
            return e.Description == null || (e.Description != null && !e.Description.Contains(keyword));
        }

        public static double EventLength(Event e)
        {
            return e.End.DateTime.Value.Subtract(e.Start.DateTime.Value).TotalMinutes;
        }

        public static double GapBetween(Event firstEvent, Event secondEvent)
        {
            return secondEvent.Start.DateTime.Value.Subtract(firstEvent.End.DateTime.Value).TotalMinutes;

        }

        public static bool StaticAndEventCollide(List<Event> staticEvents, Event e)
        {
            bool unique = true;
            int count = 0;
            while (unique && count < staticEvents.Count)
            {
                if (!DistinctEvents(e, staticEvents[count]))
                    unique = false;
                count++;
            }
            return unique;
        }


        public static List<Event> UpdateEventsBackward(List<Event> events, int offset)
        {
            foreach (Event e in events)
            {
                if (StartedYet(e.Start.DateTime.Value))
                    e.End.DateTime = e.End.DateTime.Value.AddMinutes(offset);
                else
                {
                    e.Start.DateTime = e.Start.DateTime.Value.AddMinutes(offset);
                    e.End.DateTime = e.End.DateTime.Value.AddMinutes(offset);
                }
            }
            return events;
        }
        public static List<Event> OnlyShortEventsToList(Events events)
        {
            List<Event> eventsList = new List<Event>(events.Items.Count);
            if (DataWasNotNull(events))
            {
                foreach (Event e in events.Items)
                {
                    if (e.Start.DateTime.HasValue)
                        eventsList.Add(e);

                }
            }
            return eventsList;
        }

        //The Time provided will be from the mEvents, which overlap the sEvents.
        public static void FindBetterTime(Event mEvent, Event sEvent)
        {
            if (MEventBeforeSEvent(mEvent, sEvent))
            {

            }
            else if (MEventAfterSEvent(mEvent, sEvent))
            {

            }
        }
    }
}
