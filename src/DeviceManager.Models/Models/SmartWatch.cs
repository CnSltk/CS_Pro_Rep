using DeviceManager.Models.Exceptions;

namespace DeviceManager.Models.Models;

public class SmartWatch : Device
{
    public string Type { get; set; } = "SmartWatch";
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public int BatteryPercentage { get; set; }

    // Parameterless constructor
    public SmartWatch() { }

    public SmartWatch(string id, string name, int batteryPercentage, byte[] rowVersion)
        : base(id, name)
    {
        Type = "SmartWatch";
        BatteryPercentage = batteryPercentage;
        RowVersion = rowVersion;
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