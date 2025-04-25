using System;
using System.Collections.Generic;
using System.IO;
using DeviceManager.Models.Models;

namespace APBD_02
{
    /// <summary>
    /// Loads devices from a source using an injected parser.
    /// </summary>
    public class DataLoader
    {
        

        public static List<Device> LoadDevicesFromFile(string filePath)
        {
            var devices = new List<Device>();

            foreach (var line in File.ReadLines(filePath))
            {
                try
                {
                    var device = DeviceParser.Parse(line);
                    devices.Add(device);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not parse line: '{line}' -> {ex.Message}");
                }
            }

            return devices;
        }
    }
}