using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Microsoft.VisualBasic.Devices;
using System.Management;

namespace HardwareUtilizationService
{
    class ServiceHost
    {
        private readonly System.Timers.Timer _timer;

        private static bool entered_data;

        private List<HardwareType> _hardwareTypes;
        private UtilizationValues _utilizationValues;

        public ServiceHost(int timerInterval)
        {
            _timer = new System.Timers.Timer(timerInterval - 500);
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            
            CreateUtilizationValuesObject();

            if (!entered_data)
            {
                _hardwareTypes = new List<HardwareType>();
                AddHardwerTypes("Win32_Processor");
                AddHardwerTypes("Win32_DiskDrive");
                SqliteDataAccess.AddData(_utilizationValues, _hardwareTypes);
                entered_data = true;
            }
        }

        private void CreateUtilizationValuesObject()
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
            Thread.Sleep(500);

            string cpuValue = cpuCounter.NextValue() + "%";

            int physicalMemoryInMB = (int)(computerInfo.TotalPhysicalMemory / 1000000);
            string memoryValue = ((physicalMemoryInMB - ramCounter.NextValue()) / physicalMemoryInMB) * 100 + "%";

            string diskValue = diskCounter.NextValue() + "%";

            _utilizationValues = new UtilizationValues(cpuValue,memoryValue,diskValue);
        }
        private void AddHardwerTypes(string searchKey)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + searchKey);
            string model = string.Empty, role = string.Empty, serial = string.Empty;

            foreach (var obj in searcher.Get())
            {
                foreach (var property in obj.Properties)
                {
                    switch (property.Name)
                    {
                        case "Name":
                            model = property.Value.ToString();
                            break;
                        case "SerialNumber":
                            serial = property.Value.ToString();
                            break;
                        case "Role":
                            role = property.Value.ToString();
                            break;
                    }
                }
                
                _hardwareTypes.Add(new HardwareType(model, $"{role}, serial number: {serial}"));
            }               
        }
        

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
    }
}
