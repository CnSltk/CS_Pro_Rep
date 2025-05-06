using DeviceManager.Models.Models;

namespace APBD_02;

public class DeviceManager
{
    private readonly List<Device> _devices;

    public IEnumerable<Device> GetAllDevices()
    {
        return _devices;
    }

    public Device? GetDeviceById(string id)
    {
        return _devices.FirstOrDefault(d => d.Id == id);
    }

    public void AddDevice(Device? device)
    {
       // if (_devices.Any(d => d.Id == device.Id)) return;
        //_devices.Add(device);
    }

    public bool UpdateDevice(string id, string newName)
    {
        var device = GetDeviceById(id);
        if (device == null) return false;

        device.Name = newName;
        return true;
    }

    public bool DeleteDevice(string id)
    {
        var device = GetDeviceById(id);
        if (device == null) return false;

        return _devices.Remove(device);
    }
}