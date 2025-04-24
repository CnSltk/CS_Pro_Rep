using DeviceManager.Models.Models;

namespace TestProject1;

public class DeviceManagerTests
{
private readonly List<Device> _devices;

public DeviceManagerTests()
{
    _devices = new List<Device>
    {
        new Smartwatch("SW-1", "Apple Watch", 50),
        new PersonalComputer("P-2", "Dell Laptop", "Windows 11"),
        new EmbeddedDevice("ED-3", "IoT Sensor", "192.168.1.10", "MD Ltd. Network")
    };
}

 [Fact]
    public void TurnOnSmartwatch_ShouldReduceBatteryAndSetStatus()
    {
        var watch = (Smartwatch)_devices[0];

        watch.TurnOn();

        Assert.Equal(40, watch.BatteryPercentage);
        Assert.True(watch.IsTurnedOn);
    }

    [Fact]
    public void TurnOffDevice_ShouldSetIsTurnedOnToFalse()
    {
        var pc = new PersonalComputer("P-8", "HP Laptop", "Linux");
        pc.TurnOn();

        pc.TurnOff();

        Assert.False(pc.IsTurnedOn);
    }

    [Fact]
    public void TurnOffDevice_WhenNotTurnedOn_ShouldNotThrow()
    {
        var watch = new Smartwatch("SW-9", "Huawei Watch", 80);

        var exception = Record.Exception(() => watch.TurnOff());

        Assert.Null(exception);
        Assert.False(watch.IsTurnedOn);
    }

    [Fact]
    public void ToString_ShouldIncludeDeviceDetails()
    {
        var watch = new Smartwatch("SW-10", "Garmin Watch", 80);

        var result = watch.ToString();

        Assert.Contains("Garmin Watch", result);
        Assert.Contains("Battery:", result);
    }

    [Fact]
    public void EmbeddedDevice_WithInvalidIp_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new EmbeddedDevice("ED-7", "Bad Device", "invalid_ip", "Net"));
    }

    [Fact]
    public void Smartwatch_ShouldInheritDevice()
    {
        var watch = new Smartwatch("SW-15", "FitBit", 60);

        Assert.IsAssignableFrom<Device>(watch);
        Assert.Equal("SW-15", watch.Id);
    }
}