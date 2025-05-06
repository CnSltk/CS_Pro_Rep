using System.Text.Json.Nodes;
using DeviceManager.Models.Models;
using Microsoft.OpenApi.Models;
using DeviceManager.Core.Interfaces;
using DeviceManager.Core.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Device API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("DeviceManager") 
                       ?? throw new InvalidOperationException("Connection string 'DeviceManager' not found.");
builder.Services.AddSingleton<IDeviceService>(_ => new DeviceService(connectionString));

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
app.MapPost("/api/devices", async (HttpRequest request, IDeviceService service) =>
{
    string? contentType = request.ContentType?.ToLower();

    if (string.IsNullOrWhiteSpace(contentType))
        return Results.BadRequest("Missing Content-Type.");

    string rawBody;
    using var reader = new StreamReader(request.Body);
    rawBody = await reader.ReadToEndAsync();

    Device? device = null;
    string generatedId = Guid.NewGuid().ToString(); 

    if (contentType == "application/json")
    {
        var json = JsonNode.Parse(rawBody);
        if (json == null) return Results.BadRequest("Invalid JSON.");

        var type = json["Type"]?.ToString();
        var name = json["Name"]?.ToString();

        if (string.IsNullOrWhiteSpace(name))
            return Results.BadRequest("Name is required.");
        switch (type)
        {
            case "SW":
                var battery = json["BatteryPercentage"]?.GetValue<int>() ?? -1;
                if (battery < 0 || battery > 100)
                    return Results.BadRequest("BatteryPercentage must be between 0-100.");
                device = new SmartWatch
                {
                    Id = generatedId,
                    Name = name,
                    BatteryPercentage = battery
                };
                break;
            case "PC":
                var os = json["OperatingSystem"]?.ToString();
                if (string.IsNullOrWhiteSpace(os))
                    return Results.BadRequest("OperatingSystem is required for PC.");
                device = new PersonalComputer
                {
                    Id = generatedId,
                    Name = name,
                    OperatingSystem = os
                };
                break;
            case "ED":
                var ip = json["IPAddress"]?.ToString();
                var net = json["NetworkName"]?.ToString();
                if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(net))
                    return Results.BadRequest("IPAddress and NetworkName are required for ED.");
                device = new EmbeddedDevice
                {
                    Id = generatedId,
                    Name = name,
                    IPAddress = ip,
                    NetworkName = net
                };
                break;
            default:
                return Results.BadRequest("Invalid Type.");
        }
    }
    else if (contentType == "text/plain")
    {
        var parts = rawBody.Split(',');
        if (parts.Length < 3)
            return Results.BadRequest("Invalid text format.");
        var name = parts[1];
        var prefix = parts[0].Substring(0, 2);

        switch (prefix) 
        {
            case "SW":
                if (parts.Length != 4)
                    return Results.BadRequest("Smartwatch requires 4 fields.");
                if (!int.TryParse(parts[3].Replace("%", ""), out int bp))
                    return Results.BadRequest("Invalid battery percentage.");
                device = new SmartWatch
                {
                    Id = generatedId,
                    Name = name,
                    BatteryPercentage = bp
                };
                break;
            case "PC":
                device = new PersonalComputer
                {
                    Id = generatedId,
                    Name = name,
                    OperatingSystem = parts.Length >= 4 ? parts[3] : "",
                };
                break;
            case "ED":
                if (parts.Length != 4)
                    return Results.BadRequest("Embedded requires 4 fields.");
                device = new EmbeddedDevice
                {
                    Id = generatedId,
                    Name = name,
                    IPAddress = parts[2],
                    NetworkName = parts[3]
                };
                break;
            default:
                return Results.BadRequest("Invalid device type prefix."); 
        }
    }
    else {
        return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);
    }

    service.AddDevice(device);
    return Results.Created($"/api/devices/{device.Id}", device);
})
.Accepts<string>("application/json", ["text/plain"]);


//PUT Update a Device by ID
app.MapPut("/api/devices/{id}", (string id, DeviceDTO dto, IDeviceService service) =>
{
    var existing = service.GetDeviceById(id);
    if (existing == null)
        return Results.NotFound();

    if (string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest("Name is required.");

    if (string.IsNullOrWhiteSpace(dto.Type))
        return Results.BadRequest("Type is required.");
    var expectedTypeName = dto.Type switch 
    { 
        "SW" => "Smartwatch", 
        "PC" => "PersonalComputer", 
        "ED" => "EmbeddedDevice", 
        _ => null 
    };
    
    if (existing.GetType().Name != expectedTypeName)
        return Results.BadRequest("Device type mismatch.");
    switch (dto.Type)
    {
        case "SW" when existing is SmartWatch sw:
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
