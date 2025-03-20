using System;
using System.Collections.Generic;
using APBD_02;
using APBD_02.Models;

class Program
{
    static void Main()
    {
        string filePath = "C:\\Users\\cansa\\RiderProjects\\APBD_02\\APBD_02\\input.txt";
        DataLoader dataLoader = new DataLoader(filePath);

        Console.WriteLine("Loading devices from file...\n");
        List<Device> devices = dataLoader.LoadDevices();

        Console.WriteLine("\nLoaded Devices:");
        foreach (var device in devices)
        {
            Console.WriteLine(device);
        }

        // ✅ Interactive Menu for Device Management
        while (true)
        {
            Console.WriteLine("\n Choose an action:");
            Console.WriteLine("1 Turn on a device");
            Console.WriteLine("2 Turn off a device");
            Console.WriteLine("3  Show all devices");
            Console.WriteLine("4 Exit");
            Console.Write("Please enter your choice: ");
            
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    TurnOnDevice(devices);
                    break;

                case "2":
                    TurnOffDevice(devices);
                    break;

                case "3":
                    ShowDevices(devices);
                    break;

                case "4":
                    Console.WriteLine("Exiting the application...");
                    return;

                default:
                    Console.WriteLine("Invalid choice! Please try again.");
                    break;
            }
        }
    }

    // ✅ Function to turn on a device
    static void TurnOnDevice(List<Device> devices)
    {
        Console.Write("Enter device name or ID to turn ON: ");
        string input = Console.ReadLine();
        Device device = FindDevice(devices, input);

        if (device == null)
        {
            Console.WriteLine("Device not found.");
            return;
        }

        try
        {
            device.TurnOn();
            Console.WriteLine($"{device.Name} is now ON!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cannot turn on device: {ex.Message}");
        }
    }

    // ✅ Function to turn off a device
    static void TurnOffDevice(List<Device> devices)
    {
        Console.Write("Enter device name or ID to turn OFF: ");
        string input = Console.ReadLine();
        Device device = FindDevice(devices, input);

        if (device == null)
        {
            Console.WriteLine("Device not found.");
            return;
        }

        device.TurnOff();
        Console.WriteLine($"{device.Name} is now OFF!");
    }

    // ✅ Function to display all devices
    static void ShowDevices(List<Device> devices)
    {
        Console.WriteLine("\nList of Devices:");
        foreach (var device in devices)
        {
            Console.WriteLine(device);
        }
    }

    // ✅ Function to find a device by name or ID
    static Device FindDevice(List<Device> devices, string input)
    {
        foreach (var device in devices)
        {
            if (device.Name.Equals(input, StringComparison.OrdinalIgnoreCase) || device.Id.ToString() == input)
            {
                return device;
            }
        }
        return null;
    }
}
