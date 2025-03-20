using System;
using System.Collections.Generic;
using APBD_02.Models;

namespace APBD_02
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "devices.txt";  // Adjust file path as needed
            DataLoader dataLoader = new DataLoader(filePath);
            List<Device> devices = dataLoader.LoadDevices();

            Console.WriteLine("=== Device Manager ===");
            foreach (var device in devices)
            {
                Console.WriteLine(device);
            }

            Console.WriteLine("\nEnter 'on [id]' to turn on a device, 'off [id]' to turn off, or 'exit' to quit:");
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input == "exit") break;

                string[] parts = input.Split(' ');
                if (parts.Length == 2 && int.TryParse(parts[1], out int id))
                {
                    Device foundDevice = devices.Find(d => d.Id == id);
                    if (foundDevice != null)
                    {
                        if (parts[0] == "on") foundDevice.TurnOn();
                        else if (parts[0] == "off") foundDevice.TurnOff();
                    }
                    else
                    {
                        Console.WriteLine("Device not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid command.");
                }
            }
        }
    }
}