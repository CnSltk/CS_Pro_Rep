using System.Data;
using DeviceManager.Data.Database;
using DeviceManager.Models.Models;
using Microsoft.Data.SqlClient;

namespace DeviceManager.Data.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DeviceRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddDeviceAsync(Device device)
    {
        await using (var connection = _connectionFactory.CreateConnection())
        await using (var command = new SqlCommand("AddDevice", connection))
        {
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Type", device.Type ?? throw new InvalidOperationException("Device.Type cannot be null"));
            command.Parameters.AddWithValue("@Id", device.Id);
            command.Parameters.AddWithValue("@Name", device.Name);
            command.Parameters.AddWithValue("@IsEnabled", device.IsTurnedOn);
            

            command.Parameters.AddWithValue("@BatteryPercentage", DBNull.Value);
            command.Parameters.AddWithValue("@OperationSystem", DBNull.Value);
            command.Parameters.AddWithValue("@IpAddress", DBNull.Value);
            command.Parameters.AddWithValue("@NetworkName", DBNull.Value);

            switch (device)
            {
                case SmartWatch sw:
                    command.Parameters["@BatteryPercentage"].Value = sw.BatteryPercentage;
                    break;

                case PersonalComputer pc:
                    command.Parameters["@OperationSystem"].Value = pc.OperatingSystem;
                    break;

                case EmbeddedDevice ed:
                    command.Parameters["@IpAddress"].Value = ed.IPAddress;
                    command.Parameters["@NetworkName"].Value = ed.NetworkName;
                    break;

                default:
                    throw new NotSupportedException($"Unknown device type: {device.Type}");
            }

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<Device?> GetDeviceByIdAsync(string id)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = new SqlCommand("GetDeviceById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        var type = reader.GetString(2);
        Device device = type switch
        {
            "ED" => new EmbeddedDevice(
                reader.GetString(0),
                reader.GetString(1),
                "0.0.0.0",
                "UnknownNetwork",
                Array.Empty<byte>()),
            "PC" => new PersonalComputer(
                reader.GetString(0),
                reader.GetString(1),
                "Windows",
                Array.Empty<byte>()),
            "SW" => new SmartWatch(
                reader.GetString(0),
                reader.GetString(1),
                100,
                Array.Empty<byte>()),
            _    => throw new NotSupportedException($"Device type '{type}' is not supported")
        };

        device.Type = type; 
        return device;
    }
    public async Task UpdateDeviceAsync(Device device)
    {
        await using (var connection = _connectionFactory.CreateConnection())
        await using (var command = new SqlCommand("UpdateDevice", connection))
        {
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Id", device.Id);
            command.Parameters.AddWithValue("@Name", device.Name);
            command.Parameters.AddWithValue("@IsEnabled", device.IsTurnedOn);
            command.Parameters.AddWithValue("@Type", device.Type);
            command.Parameters.AddWithValue("@BatteryPercentage", DBNull.Value);
            command.Parameters.AddWithValue("@OperationSystem", DBNull.Value);
            command.Parameters.AddWithValue("@IpAddress", DBNull.Value);
            command.Parameters.AddWithValue("@NetworkName", DBNull.Value);
            switch (device)
            {
                case SmartWatch sw:
                    command.Parameters["@BatteryPercentage"].Value = sw.BatteryPercentage;
                    break;
                case PersonalComputer pc:
                    command.Parameters["@OperationSystem"].Value = pc.OperatingSystem;
                    break;
                case EmbeddedDevice ed:
                    command.Parameters["@IpAddress"].Value = ed.IPAddress;
                    command.Parameters["@NetworkName"].Value = ed.NetworkName;
                    break;
            }
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteDeviceAsync(string id, string type)
    {
        await using (var connection = _connectionFactory.CreateConnection())
        await using (var command = new SqlCommand("DeleteDevice", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Type", type);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
    public async Task<List<Device>> GetAllAsync()
    {
        await using (var connection = _connectionFactory.CreateConnection())
        await using (var command = new SqlCommand("GetAllDevices", connection))
        {
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            var list = new List<Device>();

            while (await reader.ReadAsync())
            {
                string type = reader.GetString(2);

                Device device = type switch
                {
                    "ED" => new EmbeddedDevice(
                        reader.GetString(0),
                        reader.GetString(1),
                        "0.0.0.0",
                        "UnknownNetwork",
                        Array.Empty<byte>()
                    ),
                    "PC" => new PersonalComputer(
                        reader.GetString(0),
                        reader.GetString(1),
                        "Windows",
                        Array.Empty<byte>()
                    ),
                    "SW" => new SmartWatch(
                        reader.GetString(0),
                        reader.GetString(1),
                        100,
                        Array.Empty<byte>()
                    ),
                    _ => throw new NotSupportedException($"Device type '{type}' is not supported")
                };
                list.Add(device);
            }
            return list;
        } } }
