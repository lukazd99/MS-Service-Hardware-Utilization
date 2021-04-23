using System;
using System.Collections.Generic;
using System.Linq;
using Topshelf;

namespace HardwareUtilizationService
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter time interval in seconds for service:");
            int interval = int.Parse(Console.ReadLine());

            // TOPSHELF LIBRARY CODE
            // Used for starting the ServiceHost class as a service and configuring it.
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<ServiceHost>(s =>
                {
                    s.ConstructUsing(hardwareUtilization => new ServiceHost(interval* 1000));
                    s.WhenStarted(hardwareUtilization => hardwareUtilization.Start());
                    s.WhenStopped(hardwareUtilization => hardwareUtilization.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("HardwareUtilizationService");
                x.SetDisplayName("NCR Demo Hardware Utilization Service");
                x.SetDescription("NCR Demo Hardware Utilization Service");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
