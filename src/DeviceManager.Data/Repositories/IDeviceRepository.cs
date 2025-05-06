using DeviceManager.Models.Models;

namespace DeviceManager.Data.Repositories;

public interface IDeviceRepository
{
    Task AddDeviceAsync(Device device);
    Task<Device?> GetDeviceByIdAsync(string id);
    Task UpdateDeviceAsync(Device device);
    Task DeleteDeviceAsync(string id,string type);
    Task<List<Device>> GetAllAsync();
}
