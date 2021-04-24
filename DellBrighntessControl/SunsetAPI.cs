using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace WinAPIBrightnessControl
{
    class SunsetAPI
    {
        SunsetAPIResponse sunsetResponse;
        private int Day;

        public SunsetAPI()

        {
            Day = DateTime.Now.Day;
            GETWeather();
        }

        /// <summary>
        ///  Sunshine duration during previous 60 minutes
        /// </summary>
        /// <returns></returns>
        private SunsetAPIResponse GetResponse
        {
            get
            {
                {
                    if (DateTime.Now.Day != Day) //Old weather.
                    {
                        GETWeather();
                        Day = DateTime.Now.Day;
                    }
                    return sunsetResponse;
                }
            }
        }

        public uint brightness
        {
            get
            {
                double x_time = DateTime.Now.Hour + DateTime.Now.Minute / 60.0;
                uint brightness = (uint)((int)((x_time - SunriseAsMinute) * (x_time - SunsetAsMinute) * (-6)));
                if (brightness > 100)
                {
                    return 100;
                }
                else if (brightness < 40)
                {
                    return 40;
                }
                else
                {
                    return brightness;
                }
            }
        }

        public double SunriseAsMinute
        {
            get
            {
                return GetResponse.results.sunrise.Hour + GetResponse.results.sunrise.Minute / 60.0;
            }
        }

        public double SunsetAsMinute
        {
            get
            {
                return GetResponse.results.sunset.Hour + GetResponse.results.sunset.Minute / 60.0;
            }
        }

        private void GETWeather()
        {
            try
            {
                //https://api.sunrise-sunset.org/json?lat=51.0612766&lng=-1.3131692&date=today&formatted=0

                string URI = "https://api.sunrise-sunset.org/json?lat=51.0612766&lng=-1.3131692&date=today&formatted=0";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URI);
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                httpWebRequest.Accept = "application/json; charset=utf-8";
                string file;
                var response = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    file = sr.ReadToEnd();
                }

                sunsetResponse = JsonConvert.DeserializeObject<SunsetAPIResponse>(file);
            }
            catch //Error. 
            {
                sunsetResponse = null;
            }
        }
    }
}
