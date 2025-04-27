using DeviceManager.Models.InterFaces;
using Microsoft.Data.SqlClient;
using DeviceManager.Models.Models;
using System.Net;

namespace APBD_02.Services
{
public class DeviceService : IDeviceService
{
private readonly string _connectionString;

public DeviceService(string connectionString)
{
    _connectionString = connectionString;
}

public IEnumerable<Device> GetAllDevices()
{
    var devices = new List<Device>();
    using var conn = new SqlConnection(_connectionString);
    conn.Open();

    var cmd = new SqlCommand(@"
        SELECT
            d.Id,
            d.Name,
            d.Type,
            e.IpAddress,
            e.NetworkName,
            pc.OperationSystem,
            sw.BatteryPercentage
        FROM Devices d
        LEFT JOIN Embedded e ON d.Id = e.DeviceId
        LEFT JOIN PersonalComputer pc ON d.Id = pc.DeviceId
        LEFT JOIN Smartwatch sw ON d.Id = sw.DeviceId
    ", conn);

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        var type = reader["Type"].ToString();
        if (type == "ED")
        {
            var ip = reader["IpAddress"] is DBNull ? "" : reader["IpAddress"].ToString();
            if (!string.IsNullOrEmpty(ip) && !IPAddress.TryParse(ip, out _))
            {
                throw new Exception($"Invalid IP address format for device {reader["Id"]}");
            }
        }

        Device? device = type switch
        {
            "SW" => new Smartwatch(
                reader["Id"].ToString()!,
                reader["Name"].ToString()!,
                reader.IsDBNull(reader.GetOrdinal("BatteryPercentage")) ? 0 : reader.GetInt32(reader.GetOrdinal("BatteryPercentage"))
            ),
            "PC" => new PersonalComputer(
                id: reader["Id"].ToString()!,
                name: reader["Name"].ToString()!,
                os: reader["OperationSystem"] is DBNull ? "" : reader["OperationSystem"].ToString()!
            ),
            "ED" => new EmbeddedDevice(
                id: reader["Id"].ToString()!,
                name: reader["Name"].ToString()!,
                ip: reader["IpAddress"] is DBNull ? "" : reader["IpAddress"].ToString()!,
                network: reader["NetworkName"] is DBNull ? "" : reader["NetworkName"].ToString()!
            ),
            _ => null
        };

        if (device != null)
            devices.Add(device);
    }

    return devices;
}


public Device? GetDeviceById(string id)
{
    using var conn = new SqlConnection(_connectionString);
    conn.Open();

    var cmd = new SqlCommand(@"
        SELECT d.Id, d.Name, d.IsEnabled, d.Type,
               s.BatteryPercentage,
               pc.OperationSystem,
               e.IPAddress, e.NetworkName
        FROM Devices d
        LEFT JOIN Smartwatch s ON d.Id = s.DeviceId
        LEFT JOIN PersonalComputer pc ON d.Id = pc.DeviceId
        LEFT JOIN Embedded e ON d.Id = e.DeviceId
        WHERE d.Id = @Id
    ", conn);

    cmd.Parameters.AddWithValue("@Id", id);

    using var reader = cmd.ExecuteReader();

    if (reader.Read())
    {
        var type = reader["Type"].ToString();

        return type switch
        {
            "SW" => new Smartwatch(
                reader["Id"].ToString()!,
                reader["Name"].ToString()!,
                reader.IsDBNull(reader.GetOrdinal("BatteryPercentage")) ? 0 : reader.GetInt32(reader.GetOrdinal("BatteryPercentage"))
            ),
            "PC" => new PersonalComputer(
                id: reader["Id"].ToString()!,
                name: reader["Name"].ToString()!,
                os: reader.IsDBNull(reader.GetOrdinal("OperationSystem")) ? "" : reader["OperationSystem"].ToString()!
            ),
            "ED" => new EmbeddedDevice(
                id: reader["Id"].ToString()!,
                name: reader["Name"].ToString()!,
                ip: reader.IsDBNull(reader.GetOrdinal("IPAddress")) ? "" : reader["IPAddress"].ToString()!,
                network: reader.IsDBNull(reader.GetOrdinal("NetworkName")) ? "" : reader["NetworkName"].ToString()!
            ),
            _ => null
        };
    }

    return null;
}

public void AddDevice(Device device)
{
    ValidateDevice(device);

    using var conn = new SqlConnection(_connectionString);
    conn.Open();

    string type = device switch
    {
        Smartwatch => "SW",
        PersonalComputer => "PC",
        EmbeddedDevice => "ED",
        _ => throw new ArgumentException("Unknown device type")
    };
    string deviceId = null;
    for (int num = 1; num <= 2000; num++)
    {
        string potentialId = $"{type}-{num}";
        
        var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Devices WHERE Id = @Id", conn);
        checkCmd.Parameters.AddWithValue("@Id", potentialId);
        int exists = (int)checkCmd.ExecuteScalar();

        if (exists == 0)
        {
            deviceId = potentialId;
            break;
        }
    }

    if (deviceId == null)
        throw new InvalidOperationException($"No available ID found for device type {type} (all 2000 IDs may be in use)");
    device.Id = deviceId;
    var cmd = new SqlCommand(
        "INSERT INTO Devices (Id, Name, IsEnabled, Type) VALUES (@Id, @Name, @IsEnabled, @Type)", conn);

    cmd.Parameters.AddWithValue("@Id", device.Id);
    cmd.Parameters.AddWithValue("@Name", device.Name);
    cmd.Parameters.AddWithValue("@IsEnabled", true);
    cmd.Parameters.AddWithValue("@Type", type);

    cmd.ExecuteNonQuery();
    
    if (device is Smartwatch sw)
    {
        var cmd2 = new SqlCommand(
            "INSERT INTO Smartwatch (BatteryPercentage, DeviceId) VALUES (@Battery, @Id)", conn);
        cmd2.Parameters.AddWithValue("@Battery", sw.BatteryPercentage);
        cmd2.Parameters.AddWithValue("@Id", device.Id);
        cmd2.ExecuteNonQuery();
    }
    else if (device is PersonalComputer pc)
    {
        var cmd2 = new SqlCommand(
            "INSERT INTO PersonalComputer (OperationSystem, DeviceId) VALUES (@OS, @Id)", conn);
        cmd2.Parameters.AddWithValue("@OS", string.IsNullOrWhiteSpace(pc.OperatingSystem) ? DBNull.Value : pc.OperatingSystem);
        cmd2.Parameters.AddWithValue("@Id", device.Id);
        cmd2.ExecuteNonQuery();
    }
    else if (device is EmbeddedDevice ed)
    {
        var cmd2 = new SqlCommand(
            "INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES (@IP, @Network, @Id)", conn);
        cmd2.Parameters.AddWithValue("@IP", string.IsNullOrWhiteSpace(ed.IPAddress) ? DBNull.Value : ed.IPAddress);
        cmd2.Parameters.AddWithValue("@Network", string.IsNullOrWhiteSpace(ed.NetworkName) ? DBNull.Value : ed.NetworkName);
        cmd2.Parameters.AddWithValue("@Id", device.Id);
        cmd2.ExecuteNonQuery();
    }
}
public bool UpdateDevice(string id, Device updated)
{
    var existing = GetDeviceById(id);
    if (existing == null)
        throw new Exception("Device not found");

    if (existing.GetType() != updated.GetType())
        throw new Exception("Device type mismatch");

    ValidateDevice(updated);

    using var conn = new SqlConnection(_connectionString);
    conn.Open();

    var transaction = conn.BeginTransaction();

    try
    {
        var cmd = new SqlCommand("UPDATE Devices SET Name = @Name WHERE Id = @Id", conn, transaction);
        cmd.Parameters.AddWithValue("@Name", updated.Name);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();

        if (updated is Smartwatch sw)
        {
            var updateCmd = new SqlCommand("UPDATE Smartwatch SET BatteryPercentage = @Battery WHERE DeviceId = @Id", conn, transaction);
            updateCmd.Parameters.AddWithValue("@Battery", sw.BatteryPercentage);
            updateCmd.Parameters.AddWithValue("@Id", id);
            updateCmd.ExecuteNonQuery();
        }
        else if (updated is PersonalComputer pc)
        {
            var updateCmd = new SqlCommand("UPDATE PersonalComputer SET OperationSystem = @OS WHERE DeviceId = @Id", conn, transaction);
            updateCmd.Parameters.AddWithValue("@OS", string.IsNullOrEmpty(pc.OperatingSystem) ? DBNull.Value : pc.OperatingSystem);
            updateCmd.Parameters.AddWithValue("@Id", id);
            updateCmd.ExecuteNonQuery();
        }
        else if (updated is EmbeddedDevice ed)
        {
            var updateCmd = new SqlCommand("UPDATE Embedded SET IpAddress = @IP, NetworkName = @Network WHERE DeviceId = @Id", conn, transaction);
            updateCmd.Parameters.AddWithValue("@IP", string.IsNullOrEmpty(ed.IPAddress) ? DBNull.Value : ed.IPAddress);
            updateCmd.Parameters.AddWithValue("@Network", string.IsNullOrEmpty(ed.NetworkName) ? DBNull.Value : ed.NetworkName);
            updateCmd.Parameters.AddWithValue("@Id", id);
            updateCmd.ExecuteNonQuery();
        }

        transaction.Commit();
        return true;
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}

public bool DeleteDevice(string id)
{
    using var conn = new SqlConnection(_connectionString);
    conn.Open();
    
    var cmd1 = new SqlCommand("DELETE FROM Smartwatch WHERE DeviceId = @Id", conn);
    var cmd2 = new SqlCommand("DELETE FROM PersonalComputer WHERE DeviceId = @Id", conn);
    var cmd3 = new SqlCommand("DELETE FROM Embedded WHERE DeviceId = @Id", conn);

    cmd1.Parameters.AddWithValue("@Id", id);
    cmd2.Parameters.AddWithValue("@Id", id);
    cmd3.Parameters.AddWithValue("@Id", id);

    cmd1.ExecuteNonQuery();
    cmd2.ExecuteNonQuery();
    cmd3.ExecuteNonQuery();
    
    var cmd = new SqlCommand("DELETE FROM Devices WHERE Id = @Id", conn);
    cmd.Parameters.AddWithValue("@Id", id);

    return cmd.ExecuteNonQuery() > 0;
}

private void ValidateDevice(Device device)
{
    if (string.IsNullOrWhiteSpace(device.Name))
        throw new ArgumentException("Device Name must be provided.");

    switch (device)
    {
        case Smartwatch sw:
            if (sw.BatteryPercentage < 0 || sw.BatteryPercentage > 100)
                throw new ArgumentException("Battery percentage must be between 0 and 100.");
            if(sw.Name.Length > 15)
                throw new ArgumentException("Name shouldn't be more than 15 characters.");
            break;
        case EmbeddedDevice ed:
            if (string.IsNullOrWhiteSpace(ed.IPAddress))
                throw new ArgumentException("IP address must be provided.");
            
            if (string.IsNullOrWhiteSpace(ed.NetworkName))
                throw new ArgumentException("Network name must be provided.");
            if (ed.NetworkName.Length > 50)
                throw new ArgumentException("Network name cannot exceed 50 characters.");
            break;

        case PersonalComputer pc when string.IsNullOrWhiteSpace(pc.OperatingSystem):
            throw new ArgumentException("Operating system must be specified for personal computers.");
    }
}
}
}
