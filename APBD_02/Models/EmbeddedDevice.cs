
using System.Text.RegularExpressions;{
    public class EmbeddedDevice
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsOn { get; set; }
        public string IpAddress { get; set; }
        public string NetworkName { get; set; }

        public void SetIpAddress(string ipAddress)
        {
            var regex = new Regex(@"^(\d{1,3}\.){3}\d{1,3}$");
            if (!regex.IsMatch(ipAddress))
                throw new ArgumentException("Invalid IP address format.");

            IpAddress = ipAddress;
        }

        public void Connect()
        {
            if (!NetworkName.Contains("MD Ltd."))
                throw new ConnectionException("Invalid network name.");

            Console.WriteLine($"Connected to {NetworkName} with IP {IpAddress}.");
        }

        public void TurnOn()
        {
            Connect(); // Connecting is part of turning on the device
            IsOn = true;
            Console.WriteLine($"{Name} is now ON.");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"{Name} is now OFF.");
        }

        public override string ToString()
        {
            return $"Embedded Device: {Name}, IP: {IpAddress}, Network: {NetworkName}, IsOn: {IsOn}";
        }
    }
}