using DeviceManager.Core.Interfaces;
using DeviceManager.Data.Database;
using DeviceManager.Data.Repositories;
using DeviceManager.Models.Models;

namespace DeviceManager.Core.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _repository;

    public DeviceService(string connectionString)
    {
        var factory = new SqlConnectionFactory(connectionString);
        _repository = new DeviceRepository(factory);
    }

    public IEnumerable<Device> GetAllDevices()
    {
        return _repository.GetAllAsync().Result;
    }

    public Device? GetDeviceById(string id)
    {
        return _repository.GetDeviceByIdAsync(id).Result;
    }

    public void AddDevice(Device device)
    {
        _repository.AddDeviceAsync(device).Wait();
    }

    public bool DeleteDevice(string id)
    {
        var existing = GetDeviceById(id);
        if (existing == null) return false;

        _repository.DeleteDeviceAsync(id, existing.Type).Wait();
        return true;
    }

    public bool UpdateDevice(string id, Device updated)
    {
        try
        {
            updated.Id = id;
            _repository.UpdateDeviceAsync(updated).Wait();
            return true;
        }
        catch
        {
            return false;
        }
    }
}