using System;

namespace MonitorBrightnessDriver
{
    public class CallDDM
    {
        private BrightskyWeatherAPI weatherReport = new BrightskyWeatherAPI();
        private string Exepath = @"C:\Program Files (x86)\Dell\Dell Display Manager\ddm.exe";
        private int DdmBrightness
        {
            get
            {

                int ddmBrightness = 40 + (int)(3 * weatherReport.GetBrightness());
                if (ddmBrightness > 100)
                {
                    ddmBrightness = 100;
                }
                return ddmBrightness;
            }

        }

        public CallDDM()
        {
            SetDDMBrightness();
        }

        public void Refresh()
        {
            if (DateTime.Now.Minute == 0)
            {
                SetDDMBrightness();
            }
        }

        private void SetDDMBrightness()
        {
            // if sunshine = 0 then brightness = 40
            //if sunshine = 60 then brightness = 1000 => 40 + brigthness
            System.Diagnostics.Process.Start(Exepath, "1:/SetBrightnessLevel " + DdmBrightness.ToString());
            System.Diagnostics.Process.Start(Exepath, "2:/ SetBrightnessLevel " + DdmBrightness.ToString());

        }
    }
}
