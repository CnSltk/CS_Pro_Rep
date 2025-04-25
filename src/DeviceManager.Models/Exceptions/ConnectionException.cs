namespace DeviceManager.Models.Exceptions
{
    public class ConnectionException : Exception
    {
        public ConnectionException() 
            : base("The device cannot connect to the network. Network must contain 'MD Ltd.'.") { }

        public ConnectionException(string message) 
            : base(message) { }
    }
}