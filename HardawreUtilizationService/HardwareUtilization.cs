using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Microsoft.VisualBasic.Devices;

namespace HardawreUtilizationService
{
    class HardwareUtilization
    {
        private readonly System.Timers.Timer _timer;

        public HardwareUtilization(int timerInterval)
        {
            _timer = new System.Timers.Timer(timerInterval - 1000);
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            string[] values = GetData();
            string localPath = @"C:\Work\TrizmaNCR\HardwareUtilizationService\MS-Service-Hardware-Utilization\HardawreUtilizationService\Test.txt";
            File.AppendAllLines(localPath, values);
        }

        public string[] GetData()
        {
            PerformanceCounter cpuCounter;
            PerformanceCounter ramCounter;
            PerformanceCounter diskCounter;

            ComputerInfo computerInfo = new ComputerInfo();

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");

            cpuCounter.NextValue();
            ramCounter.NextValue();
            diskCounter.NextValue();
            Thread.Sleep(1000);

            string cpuValue = cpuCounter.NextValue() + "%";

            int physicalMemoryInMB = (int)(computerInfo.TotalPhysicalMemory / 1000000);
            string memoryValue = ((physicalMemoryInMB - ramCounter.NextValue()) / physicalMemoryInMB) * 100 + "%";

            string diskValue = diskCounter.NextValue() + "%";

            return new string[]
            {
                "CPU: " + cpuValue,
                "Memory: " + memoryValue,
                "DISK: " + diskValue,
                "Datetime: " + System.DateTime.Now.ToString(),
                "____________________________________________"
            };
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
    }
}
