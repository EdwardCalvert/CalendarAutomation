using System;

namespace TimeMethods
{
    public static class Class1
    {

        private static string CurrentTime
        {
            get
            {

                int hh = DateTime.Now.Hour;
                int mm = DateTime.Now.Minute;

                string time = "";

                if (hh < 10)
                {
                    time += "0" + hh;

                }
                else
                {
                    time += hh;
                }
                time += ":";
                if (mm < 10)
                {
                    time += "0" + mm;

                }
                else
                {
                    time += mm;
                }
                return time;
            }
        }
    }
}
