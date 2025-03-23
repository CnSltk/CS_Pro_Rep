using System.Collections.Generic;
using APBD_02.Models;

namespace APBD_02
{
    /// <summary>
    /// Factory for creating a DeviceManager instance.
    /// </summary>
    public static class DeviceManagerFactory
    {
        public static DeviceManager Create(List<Device> devices)
        {
            return new DeviceManager(devices);
        }
    }
}