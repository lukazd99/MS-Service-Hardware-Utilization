using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Timers;
using Microsoft.VisualBasic.Devices;
using System.Management;

namespace HardwareUtilizationService
{
    class ServiceHost
    {
        private readonly System.Timers.Timer _timer;

        private List<HardwareType> _hardwareTypes;
        private UtilizationValues _utilizationValues;

        public ServiceHost(int timerInterval)
        {
            // The timer interval is decreased by 500 miliseconds because there is a pause for the same time later in the code.
            _timer = new System.Timers.Timer(timerInterval - 500);
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Getting device hardware data...");

            // Adding data to the _utilizationValues object.
            CreateUtilizationValuesObject();
            
            Console.WriteLine("Success!");

            // Clearing the global list _hardwareTypes before entering new data into it.
            _hardwareTypes.Clear();

            // Adding data to the _hardwareTypes object.
            AddHardwerTypes();

            // Sending data to the database here
            SqliteDataAccess.AddData(_utilizationValues, _hardwareTypes);
        }

        // Obtains utilization data from the device CPU, Memory and DISK.
        // Stores them in the global variables of this class.
        private void CreateUtilizationValuesObject()
        {
            PerformanceCounter cpuCounter;
            PerformanceCounter ramCounter;
            PerformanceCounter diskCounter;

            ComputerInfo computerInfo = new ComputerInfo();

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");

            // Since NextValue() returns 0 the first time its called,
            // we use Thread.Sleep() for it to give us an actual value later in the code.
            cpuCounter.NextValue();
            ramCounter.NextValue();
            diskCounter.NextValue();
            Thread.Sleep(500);

            // Storing string values of every component and adding them to the _utilizationValues field.
            string cpuValue = cpuCounter.NextValue() + "%";

            int physicalMemoryInMB = (int)(computerInfo.TotalPhysicalMemory / 1024000);
            string memoryValue = ((physicalMemoryInMB - ramCounter.NextValue()) / physicalMemoryInMB) * 100 + "%";

            string diskValue = diskCounter.NextValue() + "%";

            _utilizationValues = new UtilizationValues(cpuValue,memoryValue,diskValue);
        }

        // Obtains information about the computer processor(s) and disk drive(s).
        // Adds each one of them as a new HardwareType object in the _hardwareTypes global list.
        private void AddHardwerTypes()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();

            // Processor searcher
            searcher.Query.QueryString = "SELECT Name, Role, SerialNumber FROM Win32_Processor";

            // Searches through all computer processors present and gets their values.
            foreach (var obj in searcher.Get())
            {
                string cpuModel = obj.Properties["Name"].Value.ToString();
                string cpuAdditionalInfo = obj.Properties["Role"].Value + ", " + obj.Properties["SerialNumber"].Value;

                _hardwareTypes.Add(new HardwareType(cpuModel, cpuAdditionalInfo));
            }

            // Disk searcher
            searcher.Query.QueryString = "SELECT Model, MediaType, SerialNumber, Size FROM Win32_DiskDrive";

            // Searches through all computer disk drives present and gets their values.
            foreach (var obj in searcher.Get())
            {
                string diskModel = obj.Properties["Model"].Value.ToString() + " " + ((ulong)obj.Properties["Size"].Value / 1024000000) + "gb";
                string diskAdditionalInfo = obj.Properties["MediaType"].Value + ", " + obj.Properties["SerialNumber"].Value;

                _hardwareTypes.Add(new HardwareType(diskModel, diskAdditionalInfo));
            }
        }
        
        /// <summary>
        /// Creates a textual report document. Not used by this application.
        /// </summary>
        private void Report()
        {
            List<string> lines = new List<string>();

            for (int deviceid = 1; deviceid <= _hardwareTypes.Count; deviceid++)
            {
                lines.Add("Value: " + deviceid + "\nCreateDate: " + DateTime.Now.ToString()
                    + "\nModel: " + _hardwareTypes[deviceid - 1].Model + "\n" + "AdditionalInfo: "
                    + _hardwareTypes[deviceid - 1].AdditionalInfo + "\n");
            }
           
            File.AppendAllLines(@".\Report.txt", lines);
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
    }
}
