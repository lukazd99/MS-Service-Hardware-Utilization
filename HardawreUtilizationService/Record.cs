using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardawreUtilizationService
{
    public class Record
    {
        public int Id { get; set; }
        public HardwareType HardwareType { get; set; }
        public int Value { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
