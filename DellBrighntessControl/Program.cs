using System;
using Topshelf;

namespace WinAPIBrightnessControl
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<BrightnessWorker>(s =>
                {
                    s.ConstructUsing(brightnessWorker => new BrightnessWorker());
                    //s.When
                    s.WhenStarted(brightnessWorker => brightnessWorker.Start());
                    s.WhenStopped(brightnessWorker => brightnessWorker.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("DellBrightnessControlService");
                x.SetDisplayName("Custom brighnes controler for DDM");
                x.SetDescription("Runs at boot. Silently launches DDM. Then configures the brightness of the monitor, uisng x^4 graph, so it is dim at sunset.");

            });

            //Convert from enum to int.
            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetType());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
