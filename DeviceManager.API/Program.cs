using System.Text.Json;
using APBD_02;
using DeviceManager.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using WebApplication1.DTO;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Device API", Version = "v1" });
});
var manager = new APBD_02.DeviceManager("../DeviceManager.Logic/input.txt");
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// GET /devices
// Retrieve a list of all devices
app.MapGet("/devices", () =>
{
    var devices = manager.GetAllDevices();
    return Results.Ok(devices);
});

// GET /devices/{id}
// Retrieve details of a device by its ID
app.MapGet("/devices/{id}", (string id) =>
{
    var device = manager.GetDeviceById(id);
    return device != null ? Results.Ok(device) : Results.NotFound();
});

// POST /devices
// Add a new device 
app.MapPost("/devices", (DeviceDTO? dto) =>
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

    manager.AddDevice(device);
    return Results.Created($"/devices/{device.Id}", device);
});

// PUT /devices/{id}
// Update the name of an existing device
app.MapPut("/devices/{id}", (string id, DeviceDTO? dto) =>
{
    if (dto == null)
        return Results.BadRequest("Request body is missing.");

    var existing = manager.GetDeviceById(id);
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

    return Results.Ok(existing);
});

// DELETE /devices/{id}
// Delete a device by ID
app.MapDelete("/devices/{id}", (string id) =>
{
    var success = manager.DeleteDevice(id);
    return success ? Results.Ok($"Device {id} deleted.") : Results.NotFound();
});

app.Run();
