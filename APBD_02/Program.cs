using APBD_02;
using APBD_02.Models;
class Program
{
    static void Main()
    {
        try
        {
            string filePath = "input.txt";
            DataLoader loader = new DataLoader(filePath);
            List<Device> devices = loader.LoadDevices();

            DeviceManager manager = DeviceManagerFactory.Create(devices);

            Console.WriteLine("\nLoaded Devices:");
            manager.ShowDevices();

            while (true)
            {
                Console.WriteLine("\n Choose an action:");
                Console.WriteLine("1 Turn on a device");
                Console.WriteLine("2 Turn off a device");
                Console.WriteLine("3 Show all devices");
                Console.WriteLine("4 Exit");
                Console.Write("Please enter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter device name or ID to turn ON: ");
                        manager.TurnOnDevice(Console.ReadLine());
                        break;

                    case "2":
                        Console.Write("Enter device name or ID to turn OFF: ");
                        manager.TurnOffDevice(Console.ReadLine());
                        break;

                    case "3":
                        manager.ShowDevices();
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
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}