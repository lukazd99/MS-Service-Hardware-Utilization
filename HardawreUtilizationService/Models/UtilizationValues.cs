namespace HardwareUtilizationService
{
    public class UtilizationValues
    {
        public string CPU { get; set; }
        public string Memory { get; set; }
        public string Disk { get; set; }

        public UtilizationValues(string cpu, string memory, string disk)
        {
            CPU = cpu;
            Memory = memory;
            Disk = disk;
        }
    }
}
