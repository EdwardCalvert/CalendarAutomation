using ArduinoTest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
namespace ArduninoBrightnessController
{


    public class PhisicalMonitorBrightnessController : IDisposable
    {
        //https://stackoverflow.com/questions/4013622/adjust-screen-brightness-using-c-sharp
        #region DllImport
        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitor")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyPhysicalMonitor(IntPtr hMonitor);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, [In] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("user32.dll")]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
        delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);
        #endregion

        private IReadOnlyCollection<MonitorInfo> Monitors { get; set; }

        public PhisicalMonitorBrightnessController()
        {
            UpdateMonitors();
        }

        #region Get & Set
        public void Set(uint brightness)
        {
            Set(brightness, true);
        }

        private void Set(uint brightness, bool refreshMonitorsIfNeeded)
        {
            bool isSomeFail = false;
            foreach (var monitor in Monitors)
            {
                uint realNewValue = (monitor.MaxValue - monitor.MinValue) * brightness / 100 + monitor.MinValue;
                if (SetMonitorBrightness(monitor.Handle, realNewValue))//Errors here? 
                {
                    monitor.CurrentValue = realNewValue;
                }
                else if (refreshMonitorsIfNeeded)
                {
                    isSomeFail = true;
                    break;
                }
            }

            if (refreshMonitorsIfNeeded && (isSomeFail || !Monitors.Any()))
            {
                UpdateMonitors();
                Set(brightness, false);
                return;
            }
        }

        public int Get()
        {
            if (!Monitors.Any())
            {
                return -1;
            }
            return (int)Monitors.Average(d => d.CurrentValue);
        }
        #endregion

        private void UpdateMonitors()
        {
            DisposeMonitors(this.Monitors);

            var monitors = new List<MonitorInfo>();
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData) =>
            {
                uint physicalMonitorsCount = 0;
                if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref physicalMonitorsCount))
                {
                    // Cannot get monitor count
                    return true;
                }

                var physicalMonitors = new PHYSICAL_MONITOR[physicalMonitorsCount];
                if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, physicalMonitorsCount, physicalMonitors))
                {
                    // Cannot get phisical monitor handle
                    return true;
                }

                foreach (PHYSICAL_MONITOR physicalMonitor in physicalMonitors)
                {
                    uint minValue = 0, currentValue = 0, maxValue = 0;
                    if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, ref minValue, ref currentValue, ref maxValue))
                    {
                        DestroyPhysicalMonitor(physicalMonitor.hPhysicalMonitor);
                        continue;
                    }

                    var info = new MonitorInfo
                    {
                        Handle = physicalMonitor.hPhysicalMonitor,
                        MinValue = minValue,
                        CurrentValue = currentValue,
                        MaxValue = maxValue,
                    };
                    monitors.Add(info);
                }

                return true;
            }, IntPtr.Zero);

            this.Monitors = monitors;
        }

        public void Dispose()
        {
            DisposeMonitors(Monitors);
            GC.SuppressFinalize(this);
        }

        private static void DisposeMonitors(IEnumerable<MonitorInfo> monitors)
        {
            if (monitors?.Any() == true)
            {
                PHYSICAL_MONITOR[] monitorArray = monitors.Select(m => new PHYSICAL_MONITOR { hPhysicalMonitor = m.Handle }).ToArray();
                DestroyPhysicalMonitors((uint)monitorArray.Length, monitorArray);
            }
        }

        #region Classes
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public class MonitorInfo
        {
            public uint MinValue { get; set; }
            public uint MaxValue { get; set; }
            public IntPtr Handle { get; set; }
            public uint CurrentValue { get; set; }
        }
        #endregion


    }

    public class ArduninoBrightnessWorker
    {
        private Timer _timer;
        private PhisicalMonitorBrightnessController phisicalMonitorBrightnessController = new PhisicalMonitorBrightnessController();
        private ArduinoUno uno;
        private RealWorldDataMonitor realWorldDataMonitor;
        private double lastBrightness = 0;

        public ArduninoBrightnessWorker()
        {
            uno = new ArduinoUno("COM3", 57600, true, 0);
            uno.pinMode(0, 2);
            _timer = new Timer(3000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
            realWorldDataMonitor = new RealWorldDataMonitor();
            TimerElapsed(null, null);

        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {

            int bitValue = uno.analogRead(0);
            realWorldDataMonitor.LogResult(bitValue);
            double average = realWorldDataMonitor.ReturnAverage();
            //Debug.WriteLine($"UNO READING{bitValue} Average {average}");
            double percent = (average / 1024.0) * 120;
            double newBrightness = percent +30;
            //Need to check if the value is different enoguh. 
            if (percent >= lastBrightness + 10 || percent <= lastBrightness - 10)
            {
                //Debug.WriteLine($"Significant Change: Data{percent} Previous Result = {lastBrightness}");
                if (newBrightness < 40)
                {
                    newBrightness = 40;
                }
                else if (newBrightness > 100)
                    {
                        newBrightness = 100;
                    }
                phisicalMonitorBrightnessController.Set((uint)newBrightness);
                lastBrightness = percent;
            }
            else
            {
                //Debug.WriteLine($"No change: Data{percent} Previous Result = {lastBrightness}");
            }


        }

        public void Start()
        {

            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }


    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello!");
            ArduninoBrightnessWorker worker = new ArduninoBrightnessWorker();
            worker.Start();
            Console.ReadLine();
        }

    }


}