public class DeviceDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public string Type { get; set; }
    
    public string? OperatingSystem { get; set; }
    public string? IPAddress { get; set; }
    public string? NetworkName { get; set; }
    public int? BatteryPercentage { get; set; }
}