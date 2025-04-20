namespace DeviceManager.Models.InterFaces;
/// <summary>
/// Notifies low battery for device
/// </summary>
public interface IPowerNotifier
{ 
    void NotifyLowBattery();
}
