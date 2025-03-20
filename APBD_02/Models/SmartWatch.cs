using System;
using APBD_02.Exceptions;

namespace APBD_02.Models
{
    public class Smartwatch : Device
    {
        public int BatteryPercentage { get; private set; }

        public Smartwatch(int id, string name, int battery) : base(id, name)
        {
            if (battery < 0 || battery > 100)
                throw new ArgumentException("Battery percentage must be between 0 and 100.");

            BatteryPercentage = battery;
        }

        public override void TurnOn()
        {
            if (BatteryPercentage < 11)
                throw new EmptyBatteryException();

            BatteryPercentage -= 10;
            base.TurnOn();
        }

        public override string ToString()
        {
            return base.ToString() + $", Battery: {BatteryPercentage}%";
        }
    }
}