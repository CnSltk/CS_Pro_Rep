using System;

namespace APBD_02.Exceptions
{
    public class EmptyBatteryException : Exception
    {
        public EmptyBatteryException() 
            : base("Battery is too low to turn on the device (must be at least 11%).") { }

        public EmptyBatteryException(string message) 
            : base(message) { }
    }
}