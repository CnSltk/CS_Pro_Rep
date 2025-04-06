using System;
using APBD_02;
using APBD_02.Models;
using APBD_02.Services;

class Program
{
    static void Main()
    {
        try
        {
            string filePath = "input.txt";
            string savePath = "output.txt";

            IDeviceParser parser = new DeviceParser();
            IDeviceLoader loader = new DataLoader(parser);
            var manager = new DeviceManager(loader, filePath);

            while (true)
            {
                Console.WriteLine("\nDEVICE MANAGER MENU:");
                manager.DisplayAllDevices();
                Console.WriteLine("1. Turn ON a device");
                Console.WriteLine("2. Turn OFF a device");
                Console.WriteLine("3. Delete a device");
                Console.WriteLine("4. Save devices to file");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");
                string? option = Console.ReadLine();

                switch (option)
                {

                    case "1":
                        Console.Write("Enter Device ID to turn ON: ");
                        string? idOn = Console.ReadLine();
                        if (!manager.TurnOnDevice(idOn))
                            Console.WriteLine("Could not turn on device.");
                        break;

                    case "2":
                        Console.Write("Enter Device ID to turn OFF: ");
                        string? idOff = Console.ReadLine();
                        if (!manager.TurnOffDevice(idOff))
                            Console.WriteLine("Could not turn off device.");
                        break;

                    case "3":
                        Console.Write("Enter Device ID to delete: ");
                        string? idDel = Console.ReadLine();
                        if (manager.DeleteById(idDel))
                            Console.WriteLine("Device deleted.");
                        else
                            Console.WriteLine("Device not found.");
                        break;

                    case "4":
                        manager.SaveDevicesToFile(savePath);
                        Console.WriteLine($"Devices saved to '{savePath}'.");
                        break;

                    case "0":
                        Console.WriteLine("Exiting. Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] {e.Message}");
        }
    }
}
