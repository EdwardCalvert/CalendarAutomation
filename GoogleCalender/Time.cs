using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCalender
{
    public class Time
    {
        public int Hour;
        public int Minute;

        public Time(int hour, int minute)
        {
            if (ValidMinute(minute))
                Minute = minute;
            else
                throw new ArgumentException("Minute wasn't acceptable");
            if (ValidHour(hour))
                Hour = hour;
            else
                throw new ArgumentException("Hour wasn't accpetable");

        }

        private bool ValidMinute(int minute)
        {
            return minute >= 0 && minute <= 60;
        }

        private bool ValidHour(int hour)
        {
            return hour >= 0 && hour <= 24;
        }
    }
}
