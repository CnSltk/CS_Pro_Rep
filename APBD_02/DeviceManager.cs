using System;
using System.Collections.Generic;
using System.IO;
using APBD_02.Models;

namespace APBD_02
{
    /// <summary>
    /// Manages a list of devices: display, find, delete, edit, turn on/off, and save.
    /// </summary>
    public class DeviceManager
    {
        private const int MaxCapacity = 15;
        private readonly List<Device> _devices;

        public DeviceManager(IDeviceLoader loader, string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Device file not found.");

            _devices = loader.LoadDevicesFromFile(filePath);
        }

        public void DisplayAllDevices()
        {
            foreach (var device in _devices)
            {
                Console.WriteLine(device);
            }
        }

        public Device? GetById(string id)
        {
            return _devices.Find(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public bool DeleteById(string id)
        {
            var device = GetById(id);
            if (device == null) return false;

            _devices.Remove(device);
            return true;
        }

        public bool Add(Device device)
        {
            if (_devices.Count >= MaxCapacity)
                throw new InvalidOperationException("Device capacity reached (15 devices max).");

            _devices.Add(device);
            return true;
        }

        public bool Update(string id, Device newDevice)
        {
            var index = _devices.FindIndex(d => d.Id == id);
            if (index == -1) return false;

            _devices[index] = newDevice;
            return true;
        }

        public bool TurnOnDevice(string id)
        {
            var device = GetById(id);
            if (device == null) return false;

            try
            {
                device.TurnOn();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return false;
            }
        }

        public bool TurnOffDevice(string id)
        {
            var device = GetById(id);
            if (device == null) return false;

            device.TurnOff();
            return true;
        }

        public void SaveDevicesToFile(string path)
        {
            var lines = new List<string>();
            foreach (var device in _devices)
            {
                lines.Add(device.ToString()); // Replace with ToFileFormat() if needed
            }

            File.WriteAllLines(path, lines);
        }
    }
}
