using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareUtilizationService
{
    public class HardwareType
    {
        public string Model { get; set; }
        public string AdditionalInfo { get; set; }

        public HardwareType(string model, string additionalInfo)
        {
            Model = model;
            AdditionalInfo = additionalInfo;
        }
    }
}
