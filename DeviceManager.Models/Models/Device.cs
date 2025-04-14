using System.Text.Json.Serialization;

namespace APBD_02.Models
{
    public abstract class Device
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public bool IsTurnedOn { get; set; }
        [JsonConstructor]
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