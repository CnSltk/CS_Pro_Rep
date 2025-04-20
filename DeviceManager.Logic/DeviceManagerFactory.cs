using System.Collections.Generic;

namespace APBD_02
{
    /// <summary>
    /// Factory for creating a DeviceManager instance.
    /// </summary>
    public static class DeviceManagerFactory
    {
        public static DeviceManager Create(string filePath)
        {
            return new DeviceManager(filePath);
        }
    }
}