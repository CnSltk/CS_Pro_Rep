using System.Text.Json;
using APBD_02.Services;
using DeviceManager.Models.InterFaces;
using DeviceManager.Models.Models;
using Microsoft.OpenApi.Models;
using WebApplication1.DTO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Device API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("DeviceManager");
builder.Services.AddSingleton<IDeviceService, DeviceService>(service => new DeviceService(connectionString));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// GET all devices
app.MapGet("/api/devices", (IDeviceService service) =>
{
    try
    {
        var devices = service.GetAllDevices();
        return Results.Ok(devices);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// GET device by ID
app.MapGet("/api/devices/{id}", (string id, IDeviceService service) =>
{
    var device = service.GetDeviceById(id);
    return device is null ? Results.NotFound() : Results.Ok(device);
});

// POST new device with JSON
app.MapPost("/api/devices/json", (DeviceDTO dto, IDeviceService service) =>
{
    if (string.IsNullOrWhiteSpace(dto.Id) || string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest("Id and Name are required.");

    if (dto.Type == "PC" && string.IsNullOrWhiteSpace(dto.OperatingSystem))
        return Results.BadRequest("OperatingSystem is required for PC.");

    if (dto.Type == "ED" && (string.IsNullOrWhiteSpace(dto.IPAddress) || string.IsNullOrWhiteSpace(dto.NetworkName)))
        return Results.BadRequest("IPAddress and NetworkName are required for ED.");

    if (dto.Type == "SW" && (dto.BatteryPercentage is < 0 or > 100))
        return Results.BadRequest("BatteryPercentage must be 0–100.");

    Device? device = dto.Type switch
    {
        "SW" => new Smartwatch { Id = dto.Id, Name = dto.Name, BatteryPercentage = dto.BatteryPercentage ?? 0 },
        "PC" => new PersonalComputer { Id = dto.Id, Name = dto.Name, OperatingSystem = dto.OperatingSystem },
        "ED" => new EmbeddedDevice { Id = dto.Id, Name = dto.Name, IPAddress = dto.IPAddress, NetworkName = dto.NetworkName },
        _ => null
    };

    if (device == null)
        return Results.BadRequest("Invalid device type.");

    service.AddDevice(device);
    return Results.Created($"/api/devices/{device.Id}", device);
});

//POST add new device with TEXT
app.MapPost("/api/devices/text", async (HttpRequest request, IDeviceService service) =>
{
    using var reader = new StreamReader(request.Body);
    var line = await reader.ReadLineAsync();

    if (string.IsNullOrWhiteSpace(line))
        return Results.BadRequest("Empty request body.");

    var parts = line.Split(',');

    if (parts.Length < 3)
        return Results.BadRequest("Insufficient data.");

    var typePrefix = parts[0].Substring(0, 2);

    Device? device = typePrefix switch
    {
        "SW" when parts.Length == 4 =>
            new Smartwatch
            {
                Id = parts[0],
                Name = parts[1],
                BatteryPercentage = int.TryParse(parts[3].Replace("%", ""), out int bat) ? bat : 0
            },

        "PC" when parts.Length >= 3 =>
            new PersonalComputer
            {
                Id = parts[0],
                Name = parts[1],
                OperatingSystem = parts.Length >= 4 ? parts[3] : ""
            },

        "ED" when parts.Length == 4 =>
            new EmbeddedDevice
            {
                Id = parts[0],
                Name = parts[1],
                IPAddress = parts[2],
                NetworkName = parts[3]
            },

        _ => null
    };

    if (device == null)
        return Results.BadRequest("Invalid format or unsupported device type.");

    service.AddDevice(device);
    return Results.Created($"/api/devices/{device.Id}", device);
});


// PUT update device
app.MapPut("/api/devices/{id}", (string id, DeviceDTO dto, IDeviceService service) =>
{
    var existing = service.GetDeviceById(id);
    if (existing == null)
        return Results.NotFound();

    if (string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest("Name is required.");

    switch (dto.Type)
    {
        case "SW" when existing is Smartwatch sw:
            if (dto.BatteryPercentage is < 0 or > 100)
                return Results.BadRequest("BatteryPercentage must be between 0 and 100.");
            sw.Name = dto.Name;
            sw.BatteryPercentage = dto.BatteryPercentage ?? sw.BatteryPercentage;
            break;

        case "PC" when existing is PersonalComputer pc:
            if (string.IsNullOrWhiteSpace(dto.OperatingSystem))
                return Results.BadRequest("OperatingSystem is required.");
            pc.Name = dto.Name;
            pc.OperatingSystem = dto.OperatingSystem;
            break;

        case "ED" when existing is EmbeddedDevice ed:
            if (string.IsNullOrWhiteSpace(dto.IPAddress) || string.IsNullOrWhiteSpace(dto.NetworkName))
                return Results.BadRequest("IPAddress and NetworkName are required.");
            ed.Name = dto.Name;
            ed.IPAddress = dto.IPAddress;
            ed.NetworkName = dto.NetworkName;
            break;

        default:
            return Results.BadRequest("Invalid device type or mismatch.");
    }

    bool updated = service.UpdateDevice(id, existing);
    return updated ? Results.Ok(existing) : Results.StatusCode(500);
});


// DELETE device
app.MapDelete("/api/devices/{id}", (string id, IDeviceService service) =>
{
    bool deleted = service.DeleteDevice(id);
    return deleted ? Results.Ok($"Device {id} deleted.") : Results.NotFound($"Device {id} not found.");
});

app.Run();
