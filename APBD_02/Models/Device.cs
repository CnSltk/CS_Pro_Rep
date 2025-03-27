using System;

namespace APBD_02.Models
{
    /// <summary>
    /// Device main class for devices
    /// </summary>
    public abstract class Device
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsTurnedOn { get; private set; }

        protected Device(string id, string name)
        {
            Id = id;
            Name = name;
            IsTurnedOn = false;
        }

        public virtual void TurnOn()
        {
            IsTurnedOn = true;
            Console.WriteLine($"{Name} is now turned on.");
        }

        public virtual void TurnOff()
        {
            IsTurnedOn = false;
            Console.WriteLine($"{Name} is now turned off.");
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Status: {(IsTurnedOn ? "On" : "Off")}";
        }
    }
}