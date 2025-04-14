using System;
using System.Text.Json.Serialization;
using APBD_02.Exceptions;
using APBD_02.InterFaces;

namespace APBD_02.Models
{
    public class Smartwatch : Device
    {
        [JsonPropertyName("batteryPercentage")]
        public int BatteryPercentage { get; set; }
        
        [JsonConstructor]
        public Smartwatch(string id, string name, int batteryPercentage)
            : base(id, name)
        {
            if (batteryPercentage < 0 || batteryPercentage > 100)
                throw new ArgumentException("Battery percentage must be between 0 and 100.");
            BatteryPercentage = batteryPercentage;
        }

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
            Console.WriteLine($"{Name} is now ON! Battery: {BatteryPercentage}%");
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