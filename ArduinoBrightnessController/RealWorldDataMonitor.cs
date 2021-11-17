using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ArduinoTest
{
    /// <summary>
    /// Designed to persist the last 10 data measurements. 
    /// It's then able to work out an average measurement (Won't deal with outliers,
    /// as data would never increase)
    /// </summary>
    public class RealWorldDataMonitor
    {
        private Dictionary<DateTime, int> _dateData = new Dictionary<DateTime, int>();
        private readonly int numberToKeep;

        private RealWorldDataMonitor(int numberOfDataPointsToPersist)
        {
            numberToKeep = numberOfDataPointsToPersist;
        }

        public RealWorldDataMonitor() : this(8)
        {

        }


        public void LogResult(int logData)
        {
            int keyValuePairs = _dateData.Count;
            //Debug.WriteLine(keyValuePairs);
            if (keyValuePairs >= numberToKeep)
            {
                var keyAndValue = _dateData.OrderBy(kvp => kvp.Key).First();
                //Debug.WriteLine("{0} => {1}", keyAndValue.Key, keyAndValue.Value);

                //DateTime first = _dateData.OrderBy(x => x.Key).Last().Key;

                //Debug.WriteLine($"Removed {first}, which has value of {_dateData[first]}");
                _dateData.Remove(keyAndValue.Key);
            }
            _dateData.Add(DateTime.Now, logData);

        }

        public double ReturnAverage()
        {
            double Average = (from pair in _dateData
                              select pair.Value).Average();
            return Average;
        }

    }
}
