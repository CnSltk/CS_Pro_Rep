using System.Text.Json.Nodes;
using DeviceManager.Models.Models;
using Microsoft.OpenApi.Models;
using DeviceManager.Core.Interfaces;
using DeviceManager.Core.Services;
using DeviceManager.Data.Database;
using DeviceManager.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Device API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("DeviceManager") 
                       ?? throw new InvalidOperationException("Connection string 'DeviceManager' not found.");
builder.Services.AddSingleton<IDeviceService>(_ => new DeviceService(connectionString));
builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();


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
    var contentType = request.ContentType?.Split(';')[0].Trim().ToLower();
    if (string.IsNullOrWhiteSpace(contentType))
        return Results.BadRequest("Missing Content-Type.");

    string rawBody;
    using (var sr = new StreamReader(request.Body))
    {
        rawBody = await sr.ReadToEndAsync();
    }

    string prefix;
    Device device;

    if (contentType == "application/json")
    {
        JsonNode? json = JsonNode.Parse(rawBody);
        if (json is null)
            return Results.BadRequest("Invalid JSON.");

        var t = json["Type"]?.GetValue<string>()?.ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(t))
            return Results.BadRequest("Type is required.");
        prefix = t;

        var name = json["Name"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(name))
            return Results.BadRequest("Name is required.");

        switch (prefix)
        {
            case "SW":
                var battery = json["BatteryPercentage"]?.GetValue<int>() ?? -1;
                if (battery < 0 || battery > 100)
                    return Results.BadRequest("BatteryPercentage must be between 0-100.");
                device = new SmartWatch("", name, battery, Array.Empty<byte>());
                break;

            case "PC":
                var os = json["OperatingSystem"]?.GetValue<string>();
                if (string.IsNullOrWhiteSpace(os))
                    return Results.BadRequest("OperatingSystem is required for PC.");
                device = new PersonalComputer("", name, os, Array.Empty<byte>());
                break;

            case "ED":
                var ip  = json["IPAddress"]?.GetValue<string>();
                var net = json["NetworkName"]?.GetValue<string>();
                if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(net))
                    return Results.BadRequest("IPAddress and NetworkName are required for ED.");
                device = new EmbeddedDevice("", name, ip, net, Array.Empty<byte>());
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

        prefix = parts[0].Substring(0, 2).ToUpperInvariant();
        var name = parts[1].Trim();

        switch (prefix)
        {
            case "SW":
                if (parts.Length != 4
                    || !int.TryParse(parts[3].TrimEnd('%'), out var bp)
                    || bp < 0 || bp > 100)
                    return Results.BadRequest("SmartWatch requires name,battery%.");
                device = new SmartWatch("", name, bp, Array.Empty<byte>());
                break;

            case "PC":
                var os = parts.Length >= 4 ? parts[3].Trim() : "";
                device = new PersonalComputer("", name, os, Array.Empty<byte>());
                break;

            case "ED":
                if (parts.Length != 4)
                    return Results.BadRequest("Embedded requires name,ip,network.");
                device = new EmbeddedDevice("", name, parts[2].Trim(), parts[3].Trim(), Array.Empty<byte>());
                break;

            default:
                return Results.BadRequest("Invalid device type prefix.");
        }
    }
    else
    {
        return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);
    }
    device.Type = prefix;
    var existingNumbers = service.GetAllDevices()
        .Select(d => d.Id)
        .Where(id => id.StartsWith(prefix + "-"))
        .Select(id => int.TryParse(id.Split('-', 2)[1], out var n) ? n : 0)
        .Where(n => n >= 1 && n <= 1000)
        .ToHashSet();

    int freeNum = Enumerable.Range(1, 1000).FirstOrDefault(n => !existingNumbers.Contains(n));
    if (freeNum == 0)
        return Results.Conflict($"No more IDs available for type {prefix}.");

    device.Id = $"{prefix}-{freeNum}";

    service.AddDevice(device);

    return Results.Created($"/api/devices/{device.Id}", device);
})
.Accepts<string>("application/json", "text/plain")
.Produces<Device>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status409Conflict)
.ProducesProblem(StatusCodes.Status415UnsupportedMediaType);


//PUT Update a Device by ID
app.MapPut("/api/devices/{id}", (string id, DeviceDTO dto, IDeviceService service) =>
{
    if (dto is null)
        return Results.BadRequest(new { error = "Request body is required." });

    var existing = service.GetDeviceById(id);
    if (existing is null)
        return Results.NotFound(new { error = $"Device '{id}' not found." });

    if (string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest(new { error = "Name is required." });

    if (string.IsNullOrWhiteSpace(dto.Type))
        return Results.BadRequest(new { error = "Type is required." });
    
    if (dto.Type == "SW" && existing is not SmartWatch ||
        dto.Type == "PC" && existing is not PersonalComputer ||
        dto.Type == "ED" && existing is not EmbeddedDevice)
    {
        return Results.BadRequest(new { error = "Device type mismatch." });
    }
    existing.Name       = dto.Name;
    existing.IsTurnedOn = dto.IsEnabled; 
    existing.Type       = dto.Type;
    
    switch (existing)
    {
        case SmartWatch sw:
            if (dto.BatteryPercentage is < 0 or > 100)
                return Results.BadRequest(new { error = "BatteryPercentage must be between 0 and 100." });
            sw.BatteryPercentage = dto.BatteryPercentage!.Value;
            break;

        case PersonalComputer pc:
            if (string.IsNullOrWhiteSpace(dto.OperatingSystem))
                return Results.BadRequest(new { error = "OperatingSystem is required for PC." });
            pc.OperatingSystem = dto.OperatingSystem!;
            break;

        case EmbeddedDevice ed:
            if (string.IsNullOrWhiteSpace(dto.IPAddress) || string.IsNullOrWhiteSpace(dto.NetworkName))
                return Results.BadRequest(new { error = "IPAddress and NetworkName are required for ED." });
            ed.IPAddress   = dto.IPAddress!;
            ed.NetworkName = dto.NetworkName!;
            break;
    }
    
    var updated = service.UpdateDevice(id, existing);
    if (!updated)
        return Results.Conflict(new { error = "Update failed or no rows affected." });

    return Results.Ok(existing);
})
.Accepts<DeviceDTO>("application/json")
.Produces<Device>(200)
.ProducesProblem(400)
.ProducesProblem(404)
.ProducesProblem(409);

//DELETE delete devices
app.MapDelete("/api/devices/{id}", async (string id, IDeviceRepository repository) =>
    {
        var existing = await repository.GetDeviceByIdAsync(id);
        if (existing is null)
            return Results.NotFound(new { error = $"Device '{id}' not found." });

        await repository.DeleteDeviceAsync(id, existing.Type);
        return Results.Ok($"Device '{id}'deleted.");
    })
    .Produces(204)
    .ProducesProblem(404)
    .ProducesProblem(500);



app.Run();
