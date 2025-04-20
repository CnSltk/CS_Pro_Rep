using DeviceManager.Models.Exceptions;

namespace DeviceManager.Models.Models
{
    public class PersonalComputer : Device
    {
        public string OperatingSystem { get; set; }
        public PersonalComputer() { }

        public PersonalComputer(string id, string name, string os) : base(id, name)
        {
            OperatingSystem = os;
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