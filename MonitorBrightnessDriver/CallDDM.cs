using System;
using System.Collections.Generic;
using System.Text;

namespace MonitorBrightnessDriver
{
    public class CallDDM
    {
        BrightskyWeatherAPI weatherReport;
        private string Exepath = @"C:\Program Files (x86)\Dell\Dell Display Manager\ddm.exe";
        private float Brightness { get; set; }
        private int PercentageBrigthness => 40 + (int)Brightness;

        public CallDDM()
        {
            weatherReport = new BrightskyWeatherAPI();
            Brightness = weatherReport.GetBrightness();
            SetDDMBrightness();
        }

        public void Refresh()
        {
            Brightness = weatherReport.GetBrightness();
            SetDDMBrightness();
        }

        private void SetDDMBrightness()
        {

            // if sunshine = 0 then brightness = 40
            //if sunshine = 60 then brightness = 1000 => 40 + brigthness
            System.Diagnostics.Process.Start(Exepath, "1:/SetBrightnessLevel " + PercentageBrigthness.ToString());
            System.Diagnostics.Process.Start(Exepath, "2:/ SetBrightnessLevel " + PercentageBrigthness.ToString());

        }
    }
}
