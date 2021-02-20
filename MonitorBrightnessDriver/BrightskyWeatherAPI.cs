using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace MonitorBrightnessDriver
{
    public  class BrightskyWeatherAPI
    {
        WeatherObject Weather;
        private int Day;

        public BrightskyWeatherAPI()
        
        {
            Day = DateTime.Now.Day;
            GETWeather();
        }

        /// <summary>
        ///  Sunshine duration during previous 60 minutes
        /// </summary>
        /// <returns></returns>
        public float GetBrightness()
        {
            if(DateTime.Now.Day != Day) //Old weather.
            {
                GETWeather();
            }
            float? sunshine = Weather.weather[DateTime.Now.Hour].Sunshine;
            if (sunshine != null)
            {
                return (float) sunshine;
            }
            else
            {
                return 0;
            }
            
        }


        private  void GETWeather()
        {
            //https://api.brightsky.dev/weather?lat=51.063202&lon=-1.308&date=2021-02-20

            string URI = "https://api.brightsky.dev/weather?lat=51.063202&lon=-1.308&date="+ DateTime.Now.Year + "-" + DateTime.Now.Month  +"-" + DateTime.Now.Day;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URI);
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            httpWebRequest.Accept = "application/json; charset=utf-8";
            string file;
            var response = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                file = sr.ReadToEnd();
            }

            Weather = JsonConvert.DeserializeObject<WeatherObject>(file);
        }
    }
}
