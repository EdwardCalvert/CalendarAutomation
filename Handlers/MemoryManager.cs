using System;
using System.Collections.Generic;

namespace Handlers
{
    public class MemoryManager
    {
        private int day = DateTime.Now.Day;
        private Dictionary<string, List<string>> commandHistory = new Dictionary<string, List<string>> { };

        public void MarkTabAsOpen(string path, string id)
        {
            if (DateTime.Now.Day == day)
            {
                if (!commandHistory.ContainsKey(id))
                {
                    commandHistory[id] = new List<string>();
                }
                commandHistory[id].Add(path);


            }
            else
            {
                commandHistory = new Dictionary<string, List<string>> { };
            }
        }
        public bool TabOpened(string path, string id)
        {

            return DateTime.Now.Day == day && commandHistory.ContainsKey(id) && commandHistory[id].Contains(path);

        }
    }
}
