using System;
using APBD_02.Models;
using APBD_02.Exceptions;

namespace APBD_02.Services
{
    /// <summary>
    /// Responsible for parsing device input lines and creating device objects.
    /// </summary>
    public class DeviceParser : IDeviceParser
    {
        public Device Parse(string line)
        {
            var parts = line.Split(',');

            if (parts.Length < 2)
                throw new ArgumentException("Line has insufficient data.");

            string id = parts[0].Trim();
            string typePrefix = id.Substring(0, 2).ToUpper();

            return typePrefix switch
            {
                "SW" => ParseSmartwatch(parts),
                "P-" => ParsePersonalComputer(parts),
                "ED" => ParseEmbeddedDevice(parts),
                _ => throw new ArgumentException($"Unknown device type: {id}")
            };
        }

        private Smartwatch ParseSmartwatch(string[] parts)
        {
            if (parts.Length < 4)
                throw new FormatException("Invalid smartwatch data.");

            string id = parts[0];
            string name = parts[1];
            int battery = int.Parse(parts[3].Replace("%", ""));

            return new Smartwatch(id, name, battery);
        }

        private PersonalComputer ParsePersonalComputer(string[] parts)
        {
            if (parts.Length < 3)
                throw new FormatException("Invalid PC data.");

            string id = parts[0];
            string name = parts[1];
            string os = parts.Length >= 4 ? parts[3] : "Unknown OS";

            return new PersonalComputer(id, name, os);
        }

        private EmbeddedDevice ParseEmbeddedDevice(string[] parts)
        {
            if (parts.Length < 4)
                throw new FormatException("Invalid embedded device data.");

            string id = parts[0];
            string name = parts[1];
            string ip = parts[2];
            string network = parts[3];

            return new EmbeddedDevice(id, name, ip, network);
        }
    }
}
