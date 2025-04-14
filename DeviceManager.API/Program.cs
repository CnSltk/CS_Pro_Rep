using System.Text.Json;
using APBD_02;
using APBD_02.Models;
using APBD_02.Services;
using DeviceLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using WebApplication1.DTO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Device API", Version = "v1" });
});


var manager = new DeviceManager("../DeviceManager.Logic/input.txt");
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
// Add a new device (data is sent in request body as JSON)
app.MapPost("/devices", ([FromBody] DeviceDTO dto) =>
{
    try
    {
        string json = JsonSerializer.Serialize(dto.Device);

        Device? concreteDevice = dto.Type switch
        {
            "SW" => JsonSerializer.Deserialize<Smartwatch>(json),
            "P"  => JsonSerializer.Deserialize<PersonalComputer>(json),
            "ED" => JsonSerializer.Deserialize<EmbeddedDevice>(json),
            _ => throw new ArgumentException("Invalid device type")
        };

        if (concreteDevice == null)
            return Results.BadRequest("Could not deserialize device");

        manager.AddDevice(concreteDevice); 
        return Results.Created($"/api/device/{concreteDevice.Id}", concreteDevice);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// PUT /devices/{id}
// Update the name of an existing device
app.MapPut("/devices/{id}", ([FromBody] DeviceDTO dto, string id) =>
{
    try
    {
        string json = JsonSerializer.Serialize(dto.Device);

        Device? updatedDevice = dto.Type switch
        {
            "SW" => JsonSerializer.Deserialize<Smartwatch>(json),
            "P"  => JsonSerializer.Deserialize<PersonalComputer>(json),
            "ED" => JsonSerializer.Deserialize<EmbeddedDevice>(json),
            _ => throw new ArgumentException("Invalid device type")
        };

        if (updatedDevice == null)
            return Results.BadRequest("Could not deserialize device");

        bool success = manager.UpdateDevice(id, updatedDevice.Name);
        return success ? Results.Ok(updatedDevice) : Results.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});


// DELETE /devices/{id}
// Delete a device by ID
app.MapDelete("/devices/{id}", (string id) =>
{
    var success = manager.DeleteDevice(id);
    return success ? Results.Ok($"Device {id} deleted.") : Results.NotFound();
});

app.Run();
