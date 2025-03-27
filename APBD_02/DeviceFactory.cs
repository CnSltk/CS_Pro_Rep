using System;
using APBD_02.Models;
using APBD_02.Exceptions;

namespace APBD_02
{
    /// <summary>
    /// Factory responsible for parsing device input lines and creating device objects.
    /// </summary>
    public static class DeviceFactory
    {
        public static Device CreateFromLine(string line)
        {
            var parts = line.Split(',');

            if (parts.Length < 2)
                throw new ArgumentException("Line has insufficient data.");

            string id = parts[0].Trim(); // e.g., SW-1, P-2, ED-3
            string typePrefix = id.Substring(0, 2).ToUpper();

            switch (typePrefix)
            {
                case "SW":
                    return CreateSmartwatch(parts);

                case "P-":
                    return CreatePersonalComputer(parts);

                case "ED":
                    return CreateEmbeddedDevice(parts);

                default:
                    throw new ArgumentException($"Unknown device type: {id}");
            }
        }

        private static Smartwatch CreateSmartwatch(string[] parts)
        {
            if (parts.Length < 4)
                throw new FormatException("Invalid smartwatch data.");

            string id = parts[0];
            string name = parts[1];
            int battery = int.Parse(parts[3].Replace("%", ""));

            return new Smartwatch(id, name, battery);
        }

        private static PersonalComputer CreatePersonalComputer(string[] parts)
        {
            if (parts.Length < 3)
                throw new FormatException("Invalid PC data.");

            string id = parts[0];
            string name = parts[1];
            string os = parts.Length >= 4 ? parts[3] : "Unknown OS";

            return new PersonalComputer(id, name, os);
        }

        private static EmbeddedDevice CreateEmbeddedDevice(string[] parts)
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
