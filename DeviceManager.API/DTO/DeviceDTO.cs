namespace WebApplication1.DTO;

public class DeviceDTO
{
    public string Type { get; set; } 
    public string Id { get; set; }
    public string Name { get; set; }
    public int? BatteryPercentage { get; set; }      
    public string OperatingSystem { get; set; }      
    public string IPAddress { get; set; }            
    public string NetworkName { get; set; }        
}