using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareUtilizationService
{
    public class Record
    {
        public int HardwareType { get; set; }
        public int Value { get; set; }
        public DateTime CreateDate { get; set; }
        public Record(int hardwareType, int value, DateTime createDate)
        {
            HardwareType = hardwareType;
            Value = value;
            CreateDate = createDate;
        }
    }
}
