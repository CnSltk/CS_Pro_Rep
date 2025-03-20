using System;
using System.Collections.Generic;
using System.IO;
using APBD_02.Models;
using APBD_02.Exceptions;

namespace APBD_02
{
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
                Console.WriteLine("File not found!");
                return devices;
            }

            string[] lines = File.ReadAllLines(_filePath);
            foreach (string line in lines)
            {
                try
                {
                    string[] parts = line.Split(',');

                    if (parts.Length < 2) continue; // Ignore corrupted lines

                    int id = int.Parse(parts[0]);
                    string name = parts[1];
                    string type = parts[2];

                    switch (type)
                    {
                        case "SW":
                            int battery = int.Parse(parts[3]);
                            devices.Add(new Smartwatch(id, name, battery));
                            break;

                        case "P":
                            string os = parts[3];
                            devices.Add(new PersonalComputer(id, name, os));
                            break;

                        case "ED":
                            string ip = parts[3];
                            string network = parts[4];
                            devices.Add(new EmbeddedDevice(id, name, ip, network));
                            break;

                        default:
                            Console.WriteLine($"Unknown device type: {type}");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error parsing line: {e.Message}");
                }
            }
            return devices;
        }
    }
}
