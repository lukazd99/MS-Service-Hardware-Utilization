using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HardawreUtilizationService
{
    class HardwareUtilization
    {
        private int cpu_utilization;
        private int disk_utilization;
        private int memory_utilization;

        private readonly Timer _timer;

        public HardwareUtilization(int timerInterval)
        {
            _timer = new Timer(timerInterval);
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            string[] lines = new string[] { System.DateTime.Now.ToString() };
            File.AppendAllLines(@"C:\Work\TrizmaNCR\HardwareUtilizationService\MS-Service-Hardware-Utilization\HardawreUtilizationService\Test.txt", lines);
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
    }
}
