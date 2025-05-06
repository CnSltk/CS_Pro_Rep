public class DeviceDTO {
    public string  Type             { get; set; } = null!;
    public string  Name             { get; set; } = null!;
    public bool    IsEnabled        { get; set; }        
    public int?    BatteryPercentage{ get; set; }
    public string? OperatingSystem  { get; set; }
    public string? IPAddress        { get; set; }
    public string? NetworkName      { get; set; }
    }
