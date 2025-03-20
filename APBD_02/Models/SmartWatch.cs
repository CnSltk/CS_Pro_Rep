
using APBD_02.Exceptions;
    public class Smartwatch : IPowerNotifier
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsOn { get; set; }
        public int BatteryPercentage { get; set; }

        public void SetBatteryPercentage(int percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Battery percentage must be between 0 and 100.");

            BatteryPercentage = percentage;
            if (BatteryPercentage < 20)
                NotifyLowBattery();
        }

        public void NotifyLowBattery()
        {
            Console.WriteLine("Warning: Battery is below 20%!");
        }

        public void TurnOn()
        {
            if (BatteryPercentage < 11)
                throw new EmptyBatteryException("Cannot turn on device with less than 11% battery.");

            IsOn = true;
            BatteryPercentage -= 10; // Reduce battery by 10% when turned on
            Console.WriteLine($"{Name} is now ON. Battery: {BatteryPercentage}%");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"{Name} is now OFF.");
        }

        public override string ToString()
        {
            return $"Smartwatch: {Name}, Battery: {BatteryPercentage}%, IsOn: {IsOn}";
        }
    }
