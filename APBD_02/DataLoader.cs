using System;
using System.Collections.Generic;
using System.IO;
using APBD_02.Models;

namespace APBD_02
{
    /// <summary>
    /// Loads data from input file
    /// </summary>
    public class DataLoader
    {
        private readonly string _filePath;

        public DataLoader(string filePath)
        {
            _filePath = filePath;
        }

        public List<Device> LoadDevices()
        {
            List<Device> devices = new List<Device>();

            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"❌ Error: File not found -> {_filePath}");
                return devices;
            }

            string[] lines = File.ReadAllLines(_filePath);

            foreach (var line in lines)
            {
                string[] parts = line.Split(',');

                if (parts.Length < 3)
                {
                    Console.WriteLine($"Skipping invalid line: {line}");
                    continue;
                }

                string type = parts[0];

                try
                {
                    switch (type[0])
                    {
                        case 'S': // Smartwatch
                            string name = parts[1];
                            bool isOn = bool.Parse(parts[2]);
                            int battery = int.Parse(parts[3].Replace("%", "")); // Remove % sign
                            devices.Add(new Smartwatch(devices.Count + 1, name, battery));
                            break;

                        case 'P': // Personal Computer
                            string pcName = parts[1];
                            bool isPcOn = bool.Parse(parts[2]);
                            string os = parts.Length > 3 ? parts[3] : "Unknown OS";
                            devices.Add(new PersonalComputer(devices.Count + 1, pcName, os));
                            break;

                        case 'E': // Embedded Device
                            string edName = parts[1];
                            string ip = parts[2];
                            string network = parts[3];
                            devices.Add(new EmbeddedDevice(devices.Count + 1, edName, ip, network));
                            break;

                        default:
                            Console.WriteLine($"Unknown device type: {type}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing line: {line}\n{ex.Message}");
                }
            }

            return devices;
        }
    }
}
