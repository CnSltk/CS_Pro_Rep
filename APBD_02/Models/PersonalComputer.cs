using APBD_02.Exceptions;
    public class PersonalComputer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsOn { get; set; }
        public string OperatingSystem { get; set; }

        public void Launch()
        {
            if (string.IsNullOrEmpty(OperatingSystem))
                throw new EmptySystemException("Operating system is not installed.");

            IsOn = true;
            Console.WriteLine($"Launching {Name} with {OperatingSystem}...");
        }

        public void TurnOn()
        {
            Launch(); 
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"{Name} is now OFF.");
        }

        public override string ToString()
        {
            return $"Personal Computer: {Name}, OS: {OperatingSystem}, IsOn: {IsOn}";
        }
    }
