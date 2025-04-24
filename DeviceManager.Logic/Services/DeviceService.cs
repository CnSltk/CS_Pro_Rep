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

        var cmd = new SqlCommand("SELECT * FROM Devices", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var type = reader["Type"].ToString();

            Device? device = type switch
            {
                "SW" => new Smartwatch(
                    reader["Id"].ToString()!,
                    reader["Name"].ToString()!,
                    Convert.ToInt32(reader["BatteryPercentage"])
                ),
                "PC" => new PersonalComputer(
                    reader["Id"].ToString()!,
                    reader["Name"].ToString()!,
                    reader["OperatingSystem"].ToString()!
                ),
                "ED" => new EmbeddedDevice(
                    reader["Id"].ToString()!,
                    reader["Name"].ToString()!,
                    reader["IPAddress"].ToString()!,
                    reader["NetworkName"].ToString()!
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

        var cmd = new SqlCommand("SELECT * FROM Devices WHERE Id = @Id", conn);
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
                    Convert.ToInt32(reader["BatteryPercentage"])
                ),
                "PC" => new PersonalComputer(
                    reader["Id"].ToString()!,
                    reader["Name"].ToString()!,
                    reader["OperatingSystem"].ToString()!
                ),
                "ED" => new EmbeddedDevice(
                    reader["Id"].ToString()!,
                    reader["Name"].ToString()!,
                    reader["IPAddress"].ToString()!,
                    reader["NetworkName"].ToString()!
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
            @"INSERT INTO Devices (Id, Name, Type, BatteryPercentage, OperatingSystem, IPAddress, NetworkName)
              VALUES (@Id, @Name, @Type, @Battery, @OS, @IP, @Network)", conn);

        cmd.Parameters.AddWithValue("@Id", device.Id);
        cmd.Parameters.AddWithValue("@Name", device.Name);

        switch (device)
        {
            case Smartwatch sw:
                cmd.Parameters.AddWithValue("@Type", "SW");
                cmd.Parameters.AddWithValue("@Battery", sw.BatteryPercentage);
                cmd.Parameters.AddWithValue("@OS", DBNull.Value);
                cmd.Parameters.AddWithValue("@IP", DBNull.Value);
                cmd.Parameters.AddWithValue("@Network", DBNull.Value);
                break;
            case PersonalComputer pc:
                cmd.Parameters.AddWithValue("@Type", "PC");
                cmd.Parameters.AddWithValue("@Battery", DBNull.Value);
                cmd.Parameters.AddWithValue("@OS", pc.OperatingSystem);
                cmd.Parameters.AddWithValue("@IP", DBNull.Value);
                cmd.Parameters.AddWithValue("@Network", DBNull.Value);
                break;
            case EmbeddedDevice ed:
                cmd.Parameters.AddWithValue("@Type", "ED");
                cmd.Parameters.AddWithValue("@Battery", DBNull.Value);
                cmd.Parameters.AddWithValue("@OS", DBNull.Value);
                cmd.Parameters.AddWithValue("@IP", ed.IPAddress);
                cmd.Parameters.AddWithValue("@Network", ed.NetworkName);
                break;
            default:
                throw new ArgumentException("Unknown device type");
        }
        cmd.ExecuteNonQuery();
    }
    public bool UpdateDevice(string id, Device updated)
    {
        ValidateDevice(updated);
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        var cmd = new SqlCommand(
            @"UPDATE Devices SET Name = @Name, BatteryPercentage = @Battery, OperatingSystem = @OS,
              IPAddress = @IP, NetworkName = @Network WHERE Id = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Name", updated.Name);
        switch (updated)
        {
            case Smartwatch sw:
                cmd.Parameters.AddWithValue("@Battery", sw.BatteryPercentage);
                cmd.Parameters.AddWithValue("@OS", DBNull.Value);
                cmd.Parameters.AddWithValue("@IP", DBNull.Value);
                cmd.Parameters.AddWithValue("@Network", DBNull.Value);
                break;
            case PersonalComputer pc:
                cmd.Parameters.AddWithValue("@Battery", DBNull.Value);
                cmd.Parameters.AddWithValue("@OS", pc.OperatingSystem);
                cmd.Parameters.AddWithValue("@IP", DBNull.Value);
                cmd.Parameters.AddWithValue("@Network", DBNull.Value);
                break;
            case EmbeddedDevice ed:
                cmd.Parameters.AddWithValue("@Battery", DBNull.Value);
                cmd.Parameters.AddWithValue("@OS", DBNull.Value);
                cmd.Parameters.AddWithValue("@IP", ed.IPAddress);
                cmd.Parameters.AddWithValue("@Network", ed.NetworkName);
                break;
            default:
                return false;
        }

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool DeleteDevice(string id)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

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

            case EmbeddedDevice ed when !IPAddress.TryParse(ed.IPAddress, out _):
                throw new ArgumentException("Invalid IP address format.");

            case EmbeddedDevice ed2 when string.IsNullOrWhiteSpace(ed2.NetworkName):
                throw new ArgumentException("Network name must be provided for embedded devices.");

            case PersonalComputer pc when string.IsNullOrWhiteSpace(pc.OperatingSystem):
                throw new ArgumentException("Operating system must be specified for personal computers.");
        }
    }
}
}
