using System;
using System.Collections.Generic;
using DeviceManager.Models.Exceptions;
using DeviceManager.Models.Models;
using Xunit;

namespace TestProject1
{
    public class DeviceManagerTests
    {
        private readonly List<Device> _devices;

        public DeviceManagerTests()
        {
            _devices = new List<Device>
            {
                new SmartWatch("SW-1", "Apple Watch", 50, Array.Empty<byte>()),
                new PersonalComputer("PC-1", "Dell Laptop", "Windows 11", Array.Empty<byte>()),
                new EmbeddedDevice("ED-1", "IoT Sensor", "192.168.1.10", "MD Ltd. Network", Array.Empty<byte>())
            };
        }

        [Fact]
        public void SmartWatch_TurnOn_ShouldReduceBatteryAndSetStatus()
        {
            var watch = (SmartWatch)_devices[0];

            watch.TurnOn();

            Assert.Equal(40, watch.BatteryPercentage); // battery -10
            Assert.True(watch.IsTurnedOn);
        }

        [Fact]
        public void SmartWatch_TurnOn_WithLowBattery_ShouldThrowEmptyBatteryException()
        {
            var watch = new SmartWatch("SW-2", "Test Watch", 5, Array.Empty<byte>());

            Assert.Throws<EmptyBatteryException>(() => watch.TurnOn());
        }

        [Fact]
        public void SmartWatch_TurnOff_WhenNotTurnedOn_ShouldNotThrow()
        {
            var watch = new SmartWatch("SW-3", "Quiet Watch", 80, Array.Empty<byte>());

            var ex = Record.Exception(() => watch.TurnOff());

            Assert.Null(ex);
            Assert.False(watch.IsTurnedOn);
        }

        [Fact]
        public void PersonalComputer_TurnOn_WithValidOS_ShouldSetIsTurnedOn()
        {
            var pc = new PersonalComputer("PC-2", "HP Laptop", "Linux", Array.Empty<byte>());

            pc.TurnOn();

            Assert.True(pc.IsTurnedOn);
        }

        [Fact]
        public void PersonalComputer_TurnOn_WithEmptyOS_ShouldThrowEmptySystemException()
        {
            var pc = new PersonalComputer("PC-3", "No OS PC", "", Array.Empty<byte>());

            Assert.Throws<EmptySystemException>(() => pc.TurnOn());
        }

        [Fact]
        public void EmbeddedDevice_TurnOn_WithValidNetwork_ShouldConnectAndTurnOn()
        {
            var ed = new EmbeddedDevice("ED-2", "Sensor", "192.168.0.100", "OfficeNet", Array.Empty<byte>());

            ed.TurnOn();

            Assert.True(ed.IsConnected);
            Assert.True(ed.IsTurnedOn);
        }

        [Fact]
        public void EmbeddedDevice_TurnOn_WithEmptyNetwork_ShouldThrowConnectionException()
        {
            var ed = new EmbeddedDevice("ED-3", "Bad Sensor", "192.168.0.101", "", Array.Empty<byte>());

            Assert.Throws<ConnectionException>(() => ed.TurnOn());
        }

        [Fact]
        public void EmbeddedDevice_TurnOff_ShouldDisconnectAndTurnOff()
        {
            var ed = new EmbeddedDevice("ED-4", "Plug", "192.168.0.102", "HomeNet", Array.Empty<byte>());
            ed.TurnOn();

            ed.TurnOff();

            Assert.False(ed.IsConnected);
            Assert.False(ed.IsTurnedOn);
        }

        [Fact]
        public void Device_ToString_ShouldIncludeNameAndStatus()
        {
            var watch = new SmartWatch("SW-5", "My Watch", 90, Array.Empty<byte>());
            watch.TurnOn();
            var s = watch.ToString();

            Assert.Contains("My Watch", s);
            Assert.Contains("Status: On", s);
            Assert.Contains("Battery: 80%", s);
        }

        [Fact]
        public void SmartWatch_ShouldInheritDeviceBaseType()
        {
            var watch = new SmartWatch("SW-6", "Inherited Watch", 70, Array.Empty<byte>());

            Assert.IsAssignableFrom<Device>(watch);
        }
    }
}
