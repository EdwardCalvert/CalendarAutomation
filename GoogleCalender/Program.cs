using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;

namespace GoogleCalendarWPF
{
    static class Program
    {



        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                new ToastContentBuilder()
     .AddText("One process is already running!")
     .AddText("Save your gosh darn computer!")
     .Show(); // Not
                //MessageBox.Show("Only one instance at a time","GoogleCalendar.exe");

            }

        }
    }
}
