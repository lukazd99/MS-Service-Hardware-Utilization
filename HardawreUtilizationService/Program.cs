﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace HardawreUtilizationService
{
    public class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<HardwareUtilization>(s =>
                {
                    s.ConstructUsing(hardwareUtilization => new HardwareUtilization(10000));
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