using System;

namespace WinAPIBrightnessControl
{
    class SunsetAPIResponse
    {
        public Results results { get; set; }
        public string status { get; set; }

        public class Results
        {
            public DateTime sunrise { get; set; }
            public DateTime sunset { get; set; }
            public DateTime solar_noon { get; set; }
            public int day_length { get; set; }
            public DateTime civil_twilight_begin { get; set; }
            public DateTime civil_twilight_end { get; set; }
            public DateTime nautical_twilight_begin { get; set; }
            public DateTime nautical_twilight_end { get; set; }
            public DateTime astronomical_twilight_begin { get; set; }
            public DateTime astronomical_twilight_end { get; set; }
            public string calendarId { get; set; }
        }

    }
}
