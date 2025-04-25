using DeviceManager.Models.Exceptions;

namespace DeviceManager.Models.Models;

public class Smartwatch : Device
{
    public int BatteryPercentage { get; set; }

    // Parameterless constructor
    public Smartwatch() { }
    public Smartwatch(string id, string name, int batteryPercentage)
        : base(id, name)
    {
        BatteryPercentage = batteryPercentage;
    }
    public override void TurnOn()
    {
        if (IsTurnedOn) return;
        if (BatteryPercentage < 10) throw new EmptyBatteryException();
        BatteryPercentage -= 10;
        base.TurnOn();
    }
    public override string ToString()
    {
        return base.ToString() + $", Battery: {BatteryPercentage}%";
    }
}