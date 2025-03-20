using Xunit;
using APBD_02.Models;
using APBD_02.Exceptions;
using System.Collections.Generic;

namespace APBD_02.Tests
{
    public class DeviceManagerTests
    {
        private readonly List<Device> _devices;

        public DeviceManagerTests()
        {
            _devices = new List<Device>
            {
                new Smartwatch(1, "Apple Watch", 50),
                new PersonalComputer(2, "Dell Laptop", "Windows 11"),
                new EmbeddedDevice(3, "IoT Sensor", "192.168.1.10", "MD Ltd. Network")
            };
        }

        // Test: Turning on a smartwatch should reduce battery by 10%
        [Fact]
        public void TurnOnSmartwatch_ShouldReduceBattery()
        {
            // Arrange
            var watch = (Smartwatch)_devices[0];

            // Act
            watch.TurnOn();

            // Assert
            Assert.Equal(40, watch.BatteryPercentage);
        }

        // Test: Turning on a smartwatch with low battery should throw exception
        [Fact]
        public void TurnOnSmartwatch_LowBattery_ShouldThrowException()
        {
            // Arrange
            var watch = new Smartwatch(4, "Samsung Watch", 5);

            // Act & Assert
            Assert.Throws<EmptyBatteryException>(() => watch.TurnOn());
        }

        // Test: Personal computer should not turn on if OS is missing
        [Fact]
        public void TurnOnPersonalComputer_NoOS_ShouldThrowException()
        {
            // Arrange
            var pc = new PersonalComputer(5, "Acer PC", "");

            // Act & Assert
            Assert.Throws<EmptySystemException>(() => pc.TurnOn());
        }

        // Test: Embedded device should not turn on if connected to the wrong network
        [Fact]
        public void TurnOnEmbeddedDevice_InvalidNetwork_ShouldThrowException()
        {
            // Arrange
            var device = new EmbeddedDevice(6, "Smart Sensor", "192.168.1.5", "Unknown Network");

            // Act & Assert
            Assert.Throws<ConnectionException>(() => device.TurnOn());
        }

        // Test: Turning off a device should change status
        [Fact]
        public void TurnOffDevice_ShouldChangeStatus()
        {
            // Arrange
            var pc = new PersonalComputer(8, "HP Laptop", "Linux");
            pc.TurnOn(); // Turn it on first

            // Act
            pc.TurnOff();

            // Assert
            Assert.False(pc.IsTurnedOn);
        }

        // Test: Ensure the correct string format for `ToString()`
        [Fact]
        public void DeviceToString_ShouldReturnCorrectFormat()
        {
            // Arrange
            var watch = new Smartwatch(1, "Garmin Watch", 80);

            // Act
            string result = watch.ToString();

            // Assert
            Assert.Contains("Garmin Watch", result);
            Assert.Contains("Battery:", result);
        }
    }
}
