using DeviceManager.Models.Models;

namespace DeviceManager.Models.InterFaces;

public interface IDeviceService
{
    IEnumerable<Device> GetAllDevices();
    Device? GetDeviceById(string id);
    void AddDevice(Device device);
    bool DeleteDevice(string id);
    bool UpdateDevice(string id, Device device);
}