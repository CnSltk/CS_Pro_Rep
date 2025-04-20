using Xunit;
using System.Collections.Generic;
using DeviceManager.Models.Exceptions;
using DeviceManager.Models.Models;

namespace APBD_02.Tests
{
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
        public void TurnOnSmartwatch_ShouldReduceBattery()
        {
            // Arrange
            var watch = (Smartwatch)_devices[0];

            // Act
            watch.TurnOn();

            // Assert
            Assert.Equal(40, watch.BatteryPercentage);
            Assert.True(watch.IsTurnedOn);
        }

        [Fact]
        public void TurnOnSmartwatch_LowBattery_ShouldThrowException()
        {
            // Arrange
            var watch = new Smartwatch("SW-4", "Samsung Watch", 5);

            // Act & Assert
            Assert.Throws<EmptyBatteryException>(() => watch.TurnOn());
        }

        [Fact]
        public void TurnOnPersonalComputer_NoOS_ShouldThrowException()
        {
            // Arrange
            var pc = new PersonalComputer("P-5", "Acer PC", "");

            // Act & Assert
            Assert.Throws<EmptySystemException>(() => pc.TurnOn());
        }

        [Fact]
        public void TurnOnEmbeddedDevice_InvalidNetwork_ShouldThrowException()
        {
            // Arrange
            var device = new EmbeddedDevice("ED-6", "Smart Sensor", "192.168.1.5", "Unknown Network");

            // Act & Assert
            Assert.Throws<ConnectionException>(() => device.TurnOn());
        }

        [Fact]
        public void TurnOffDevice_ShouldChangeStatus()
        {
            // Arrange
            var pc = new PersonalComputer("P-8", "HP Laptop", "Linux");
            pc.TurnOn();

            // Act
            pc.TurnOff();

            // Assert
            Assert.False(pc.IsTurnedOn);
        }

        [Fact]
        public void DeviceToString_ShouldReturnCorrectFormat()
        {
            // Arrange
            var watch = new Smartwatch("SW-10", "Garmin Watch", 80);

            // Act
            string result = watch.ToString();

            // Assert
            Assert.Contains("Garmin Watch", result);
            Assert.Contains("Battery:", result);
        }
    }
}
