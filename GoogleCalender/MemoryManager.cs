using System;
using System.Collections.Generic;

namespace GoogleCalendar
{
    public class MemoryManager<T>
    {
        private int day = DateTime.Now.Day;
        private Dictionary<string, List<T>> commandHistory = new Dictionary<string, List<T>> { };

        public void MarkTabAsOpen(T obj, string id)
        {
            if (DateTime.Now.Day == day)
            {
                if (!commandHistory.ContainsKey(id))
                {
                    commandHistory[id] = new List<T>();
                }
                commandHistory[id].Add(obj);


            }
            else
            {
                commandHistory = new Dictionary<string, List<T>> { };
            }
        }
        public bool TabOpened(T obj, string id)
        {

            return DateTime.Now.Day == day && commandHistory.ContainsKey(id) && commandHistory[id].Contains(obj);

        }

    }
}