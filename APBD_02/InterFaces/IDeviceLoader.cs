using System.Collections.Generic;
using APBD_02.Models;

namespace APBD_02
{
    public interface IDeviceLoader
    {
        List<Device> LoadDevicesFromFile(string filePath);
    }
}