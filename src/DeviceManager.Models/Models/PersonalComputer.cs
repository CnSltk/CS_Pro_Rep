using DeviceManager.Models.Exceptions;

namespace DeviceManager.Models.Models
{
    public class PersonalComputer : Device
    {
        public string Type { get; set; } = "PC";
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public string OperatingSystem { get; set; }

        public PersonalComputer() { }

        public PersonalComputer(string id, string name, string os, byte[] rowVersion)
            : base(id, name)
        {
            OperatingSystem = os;
            Type = "PC";
            RowVersion = rowVersion;
        }

        public override void TurnOn()
        {
            Console.WriteLine($"Attempting to turn on {Name} with OS: {OperatingSystem}");
            if (string.IsNullOrEmpty(OperatingSystem))
                throw new EmptySystemException();

            base.TurnOn();
        }

        public override string ToString()
        {
            return base.ToString() + $", OS: {OperatingSystem}";
        }
    }
}