using System.Collections.Generic;
using APBD_02.Models;

namespace APBD_02
{
    /// <summary>
    /// Factory for creating a DeviceManager instance.
    /// </summary>
    public static class DeviceManagerFactory
    {
        public static DeviceManager Create(IDeviceLoader loader, string filePath)
        {
            return new DeviceManager(loader, filePath);
        }
    }
}