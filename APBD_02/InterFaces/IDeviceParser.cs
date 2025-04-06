using APBD_02.Models;

namespace APBD_02.Services
{
    public interface IDeviceParser
    {
        Device Parse(string line);
    }
}