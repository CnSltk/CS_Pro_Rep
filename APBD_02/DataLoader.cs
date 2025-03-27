using System;
using System.Collections.Generic;
using System.IO;
using APBD_02.Models;

namespace APBD_02
{
    /// <summary>
    /// Loads devices from a specified file path.
    /// Responsible only for reading lines and passing them to the factory.
    /// </summary>
    public class DataLoader
    {
        private readonly string _filePath;

        public DataLoader(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Loads and parses devices from the input file.
        /// </summary>
        /// <returns>A list of valid devices</returns>
        public List<Device> LoadDevices()
        {
            var devices = new List<Device>();

            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"❌ Error: File not found -> {_filePath}");
                return devices;
            }

            string[] lines = File.ReadAllLines(_filePath);

            foreach (var line in lines)
            {
                try
                {
                    var device = DeviceFactory.CreateFromLine(line);
                    if (device != null)
                        devices.Add(device);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing line: {line}\n{ex.Message}");
                }
            }

            return devices;
        }
    }
}