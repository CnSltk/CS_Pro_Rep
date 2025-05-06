using System.Text.RegularExpressions;
using DeviceManager.Models.Exceptions;

namespace DeviceManager.Models.Models
{
    public class EmbeddedDevice : Device
    {
        public string Type { get; set; } = "Embedded";
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public string IPAddress { get; set; }
        public string NetworkName { get; set; }
        public bool IsConnected { get; set; }

        public EmbeddedDevice() { }

        public EmbeddedDevice(string id, string name, string ip, string network, byte[] rowVersion)
            : base(id, name)
        {
            if (!Regex.IsMatch(ip, @"^(\d{1,3}\.){3}\d{1,3}$"))
                throw new ArgumentException("Invalid IP address format.");

            IPAddress = ip;
            NetworkName = network;
            RowVersion = rowVersion;
        }

        public override void TurnOn()
        {
            Console.WriteLine($"Attempting to connect {Name} to network {NetworkName}...");

            if (string.IsNullOrEmpty(NetworkName))
                throw new ConnectionException("Network name cannot be empty.");

            IsConnected = true;
            base.TurnOn();
        }

        public override void TurnOff()
        {
            if (IsConnected)
            {
                Console.WriteLine($"{Name} disconnected from network {NetworkName}.");
                IsConnected = false;
            }

            base.TurnOff();
        }

        public override string ToString()
        {
            return base.ToString() + $", IP: {IPAddress}, Network: {NetworkName}, Connected: {IsConnected}";
        }
    }
}