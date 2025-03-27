using APBD_02.Exceptions;
using APBD_02.InterFaces;

namespace APBD_02.Models
{
    public class Smartwatch : Device, IPowerNotifier
    {
        public int BatteryPercentage { get; private set; }

        public Smartwatch(string id, string name, int battery) : base(id, name)
        {
            if (battery < 0 || battery > 100)
                throw new ArgumentException("Battery percentage must be between 0 and 100.");

            BatteryPercentage = battery;
        }
/// <summary>
/// turns on a SmartWatch, if the batter is less than 20 gives notification about low batter. If device is already ON,
/// tells user. Every Turn on takes away 10% battery.
/// </summary>
/// <exception cref="EmptyBatteryException">tells user the battery is empty if the batter is below 10%</exception>
        public override void TurnOn()
        {
            if (IsTurnedOn)
            {
                Console.WriteLine($"{Name} is already ON!");
                return;
            }

            Console.WriteLine($"Attempting to turn on {Name} with {BatteryPercentage}% battery...");

            if (BatteryPercentage < 10)
                throw new EmptyBatteryException();

            if (BatteryPercentage < 20)
                NotifyLowBattery();

            BatteryPercentage -= 10;
            base.TurnOn();
            Console.WriteLine($"{Name} is now ON! 🔋 Battery: {BatteryPercentage}%");
        }

        public override void TurnOff()
        {
            if (!IsTurnedOn)
            {
                Console.WriteLine($"{Name} is already OFF!");
                return;
            }

            base.TurnOff();
            Console.WriteLine($"{Name} is now OFF.");
        }

        public void NotifyLowBattery()
        {
            Console.WriteLine($"Warning: {Name} has LOW BATTERY! Battery: {BatteryPercentage}%");
        }

        public override string ToString()
        {
            return base.ToString() + $", Battery: {BatteryPercentage}%";
        }
    }
}