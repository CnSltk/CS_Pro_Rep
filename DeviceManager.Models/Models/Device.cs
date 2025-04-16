namespace APBD_02.Models
{
    public abstract class Device
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsTurnedOn { get; set; }
        protected Device() { }

        protected Device(string id, string name)
        {
            Id = id;
            Name = name;
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