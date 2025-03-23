using APBD_02.Models;

namespace APBD_02
{
    /// <summary>
    /// Manages a list of devices and allows operations on them
    /// </summary>
    public class DeviceManager
    {
        private readonly List<Device> _devices;
        public DeviceManager(List<Device> devices)
        {
            _devices = devices;
        }

        public void ShowDevices()
        {
            Console.WriteLine("Devices:");
            foreach (var device in _devices)
            {
                Console.WriteLine(device);
            }
        }

        public void TurnOnDevice(string input)
        {
            var device = FindDevice(input);
            if (device==null)
            {
                Console.WriteLine("Device not found");
                return;
            }

            try
            {
                device.TurnOn();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot turn on device: {ex.Message}");
            }
        }

        public void TurnOffDevice(string input)
        {
            var device = FindDevice(input);
            {
                Console.WriteLine("Device not found.");
                return;
            }

            device.TurnOff();
        }
        private Device FindDevice(string input)
        {
            
            {
                foreach (var device in _devices)
                {
                    if (device.Name.Equals(input, StringComparison.OrdinalIgnoreCase) || device.Id.ToString() == input)
                        return device;
                }
                return null;
            }
        }
    }
}

