using System;
using System.Text.RegularExpressions;
using APBD_02.Exceptions;

namespace APBD_02.Models
{
    public class EmbeddedDevice : Device
    {
        public string IPAddress { get; private set; }
        public string NetworkName { get; private set; }

        public EmbeddedDevice(int id, string name, string ip, string network) : base(id, name)
        {
            if (!Regex.IsMatch(ip, @"^(\d{1,3}\.){3}\d{1,3}$"))
                throw new ArgumentException("Invalid IP address format.");

            IPAddress = ip;
            NetworkName = network;
        }

        public override void TurnOn()
        {
            if (!NetworkName.Contains("MD Ltd."))
                throw new ConnectionException();

            base.TurnOn();
        }

        public override string ToString()
        {
            return base.ToString() + $", IP: {IPAddress}, Network: {NetworkName}";
        }
    }
}