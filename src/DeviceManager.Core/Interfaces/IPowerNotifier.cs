namespace DeviceManager.Core.Interfaces;
/// <summary>
/// Notifies low battery for device
/// </summary>
public interface IPowerNotifier
{ 
    void NotifyLowBattery();
}
