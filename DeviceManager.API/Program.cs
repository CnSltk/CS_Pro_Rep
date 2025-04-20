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
builder.Services.AddSingleton<IDeviceService,DeviceService>(deviceService => new DeviceService(connectionString));
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

// POST new device
app.MapPost("/api/devices", (DeviceDTO? dto, IDeviceService service) =>
{
    if (dto == null)
        return Results.BadRequest("Request body is missing.");

    Device? device = dto.Type switch
    {
        "SW" => new Smartwatch
        {
            Id = dto.Id,
            Name = dto.Name,
            BatteryPercentage = dto.BatteryPercentage ?? 0
        },
        "PC" => new PersonalComputer
        {
            Id = dto.Id,
            Name = dto.Name,
            OperatingSystem = dto.OperatingSystem
        },
        "ED" => new EmbeddedDevice
        {
            Id = dto.Id,
            Name = dto.Name,
            IPAddress = dto.IPAddress,
            NetworkName = dto.NetworkName
        },
        _ => null
    };

    if (device == null)
        return Results.BadRequest("Invalid device type.");

    service.AddDevice(device);
    return Results.Created($"/api/devices/{device.Id}", device);
});

// PUT update device
app.MapPut("/api/devices/{id}", (string id, DeviceDTO? dto, IDeviceService service) =>
{
    if (dto == null)
        return Results.BadRequest("Request body is missing.");

    var existing = service.GetDeviceById(id);
    if (existing == null)
        return Results.NotFound();

    switch (dto.Type)
    {
        case "SW" when existing is Smartwatch sw:
            sw.Name = dto.Name;
            if (dto.BatteryPercentage != null)
                sw.BatteryPercentage = dto.BatteryPercentage.Value;
            break;
        case "PC" when existing is PersonalComputer pc:
            pc.Name = dto.Name;
            pc.OperatingSystem = dto.OperatingSystem;
            break;
        case "ED" when existing is EmbeddedDevice ed:
            ed.Name = dto.Name;
            ed.IPAddress = dto.IPAddress;
            ed.NetworkName = dto.NetworkName;
            break;
        default:
            return Results.BadRequest("Invalid device type or type mismatch.");
    }

    bool success = service.UpdateDevice(id, existing);
    return success ? Results.Ok(existing) : Results.StatusCode(500);
});

// DELETE device
app.MapDelete("/api/devices/{id}", (string id, IDeviceService service) =>
{
    bool deleted = service.DeleteDevice(id);
    return deleted ? Results.Ok($"Device {id} deleted.") : Results.NotFound($"Device {id} not found.");
});

app.Run();
