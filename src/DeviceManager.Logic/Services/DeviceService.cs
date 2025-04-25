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
    d.IsEnabled,
    d.Type,
    s.BatteryPercentage,
    pc.OperationSystem,
    e.IPAddress,
    e.NetworkName
  FROM Devices d
  LEFT JOIN Smartwatch s         ON s.DeviceId = d.Id
  LEFT JOIN PersonalComputer pc  ON pc.DeviceId = d.Id
  LEFT JOIN Embedded e           ON e.DeviceId = d.Id
", conn);

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        var type = reader["Type"]?.ToString();
        Device? device = type switch
        {
            "SW" => new Smartwatch(
                reader["Id"].ToString()!,
                reader["Name"].ToString()!,
                reader.GetInt32(reader.GetOrdinal("BatteryPercentage"))
            ),
            "PC" => new PersonalComputer(
                reader["Id"].ToString()!,
                reader["Name"].ToString()!,
                reader["OperationSystem"].ToString()!
            ),
            "ED" => new EmbeddedDevice(
                reader["Id"].ToString()!,
                reader["Name"].ToString()!,
                reader["IPAddress"].ToString()!,
                reader["NetworkName"].ToString()!
            ),
            _    => null
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
    SELECT d.Id, d.Name, d.IsEnabled, 
           'SW' AS Type, s.BatteryPercentage, NULL AS OperatingSystem, NULL AS IPAddress, NULL AS NetworkName
    FROM Devices d
    LEFT JOIN Smartwatch s ON s.DeviceId = d.Id
    WHERE s.DeviceId IS NOT NULL

    UNION

    SELECT d.Id, d.Name, d.IsEnabled, 
           'PC' AS Type, NULL AS BatteryPercentage, pc.OperationSystem, NULL AS IPAddress, NULL AS NetworkName
    FROM Devices d
    LEFT JOIN PersonalComputer pc ON pc.DeviceId = d.Id
    WHERE pc.DeviceId IS NOT NULL

    UNION

    SELECT d.Id, d.Name, d.IsEnabled, 
           'ED' AS Type, NULL AS BatteryPercentage, NULL AS OperatingSystem, e.IpAddress, e.NetworkName
    FROM Devices d
    LEFT JOIN Embedded e ON e.DeviceId = d.Id
    WHERE e.DeviceId IS NOT NULL
", conn);

    cmd.Parameters.AddWithValue("@Id", id);

    using var reader = cmd.ExecuteReader();

    if (reader.Read())
    {
        var type = reader["Type"]?.ToString();
        return type switch
        {
            "SW" => new Smartwatch(
                reader["Id"].ToString()!,
                reader["Name"].ToString()!,
                Convert.ToInt32(reader["BatteryPercentage"])
            ),
            "PC" => new PersonalComputer(
                reader["Id"].ToString()!,
                reader["Name"].ToString()!,
                reader["OperatingSystem"]?.ToString() ?? ""
            ),
            "ED" => new EmbeddedDevice(
                reader["Id"].ToString()!,
                reader["Name"].ToString()!,
                reader.IsDBNull(reader.GetOrdinal("IPAddress")) ? "" : reader["IPAddress"].ToString()!,
                reader.IsDBNull(reader.GetOrdinal("NetworkName")) ? "" : reader["NetworkName"].ToString()!
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
    
    var cmd = new SqlCommand(
        "INSERT INTO Devices (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)", conn);

    cmd.Parameters.AddWithValue("@Id", device.Id);
    cmd.Parameters.AddWithValue("@Name", device.Name);
    cmd.Parameters.AddWithValue("@IsEnabled", true);
    string type = device switch
    {
        Smartwatch => "SW",
        PersonalComputer => "PC",
        EmbeddedDevice => "ED",
        _ => throw new ArgumentException("Unknown device type")
    };
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
        cmd2.Parameters.AddWithValue("@OS", pc.OperatingSystem ?? (object)DBNull.Value);
        cmd2.Parameters.AddWithValue("@Id", device.Id);
        cmd2.ExecuteNonQuery();
    }
    else if (device is EmbeddedDevice ed)
    {
        var cmd2 = new SqlCommand(
            "INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES (@IP, @Network, @Id)", conn);
        cmd2.Parameters.AddWithValue("@IP", ed.IPAddress);
        cmd2.Parameters.AddWithValue("@Network", ed.NetworkName);
        cmd2.Parameters.AddWithValue("@Id", device.Id);
        cmd2.ExecuteNonQuery();
    }
    else
    {
        throw new ArgumentException("Unknown device type");
    }
}

public bool UpdateDevice(string id, Device updated)
{
    ValidateDevice(updated);

    using var conn = new SqlConnection(_connectionString);
    conn.Open();

    var cmd = new SqlCommand("UPDATE Devices SET Name = @Name WHERE Id = @Id", conn);
    cmd.Parameters.AddWithValue("@Id", id);
    cmd.Parameters.AddWithValue("@Name", updated.Name);
    cmd.ExecuteNonQuery();

    if (updated is Smartwatch sw)
    {
        var updateCmd = new SqlCommand("UPDATE Smartwatch SET BatteryPercentage = @Battery WHERE DeviceId = @Id", conn);
        updateCmd.Parameters.AddWithValue("@Battery", sw.BatteryPercentage);
        updateCmd.Parameters.AddWithValue("@Id", id);
        return updateCmd.ExecuteNonQuery() > 0;
    }
    else if (updated is PersonalComputer pc)
    {
        var updateCmd = new SqlCommand("UPDATE PersonalComputer SET OperationSystem = @OS WHERE DeviceId = @Id", conn);
        updateCmd.Parameters.AddWithValue("@OS", pc.OperatingSystem);
        updateCmd.Parameters.AddWithValue("@Id", id);
        return updateCmd.ExecuteNonQuery() > 0;
    }
    else if (updated is EmbeddedDevice ed)
    {
        var updateCmd = new SqlCommand("UPDATE Embedded SET IpAddress = @IP, NetworkName = @Network WHERE DeviceId = @Id", conn);
        updateCmd.Parameters.AddWithValue("@IP", ed.IPAddress);
        updateCmd.Parameters.AddWithValue("@Network", ed.NetworkName);
        updateCmd.Parameters.AddWithValue("@Id", id);
        return updateCmd.ExecuteNonQuery() > 0;
    }

    return false;
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
    if (string.IsNullOrWhiteSpace(device.Id) || string.IsNullOrWhiteSpace(device.Name))
        throw new ArgumentException("Device ID and Name must be provided.");

    switch (device)
    {
        case Smartwatch sw when sw.BatteryPercentage < 0 || sw.BatteryPercentage > 100:
            throw new ArgumentException("Battery percentage must be between 0 and 100.");

        case EmbeddedDevice ed when string.IsNullOrWhiteSpace(ed.IPAddress) || !IPAddress.TryParse(ed.IPAddress, out _):
            throw new ArgumentException("Invalid or missing IP address format.");
        
        case EmbeddedDevice ed2 when string.IsNullOrWhiteSpace(ed2.NetworkName):
            throw new ArgumentException("Network name must be provided for embedded devices.");

        case PersonalComputer pc when string.IsNullOrWhiteSpace(pc.OperatingSystem):
            throw new ArgumentException("Operating system must be specified for personal computers.");
    }
}
}
}
