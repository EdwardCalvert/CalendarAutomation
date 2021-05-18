using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalendar
{
    public struct Settings
    {
        public bool DebugMode;
        public bool Notifications;
        public bool LaunchURLs;
        public bool GoogleCalendarSync;
        public bool LaunchStartedURLsAfterReboot;
        public bool MonitorBrightnessControl;
        public DateTime StartWorkHour;
        public DateTime FinishtWorkHour;
        public int PullForwardTime;


    }
}
