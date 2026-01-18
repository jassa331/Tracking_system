using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Service;

[ApiController]
[Route("api/address")]
public class AddressController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IpLocationService _ipService;

    public AddressController(AppDbContext context, IpLocationService ipService)
    {
        _context = context;
        _ipService = ipService;
    }
    [ApiLog]

    [HttpPost("save")]
    public async Task<IActionResult> SaveAddress([FromBody] PatientAddress model)
    {
        string ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (ip == "::1")
            ip = "8.8.8.8"; // testing

        model.IpAddress = ip;

        // If city not provided, fallback to IP city
        if (string.IsNullOrEmpty(model.City))
        {
            model.City = await _ipService.GetCityFromIp(ip);
        }

        _context.PatientAddresses.Add(model);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Patient address saved securely",
            model.Id
        });
    }
    private string GetClientIp(HttpContext context)
    {
        string? ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrEmpty(ip))
        {
            // If multiple IPs, take the first one (real client)
            return ip.Split(',')[0];
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyLocation()
    {
        string ip = GetClientIp(HttpContext);

        if (ip == "192.168.15.63"
          //  || ip.StartsWith("192.168") || ip.StartsWith("10.")
            )
        {
            return BadRequest(new
            {
                message = "Local/private IP detected",
                ip
            });
        }

        var location = await _ipService.GetLocationAsync(ip);

        return Ok(new
        {
            ip,
            location?.City,
            location?.RegionName,
            location?.Country,
            location?.Lat,
            location?.Lon
        });
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return BadRequest("IP address is required");

        var result = await _ipService.GetLocationAsync(ip);

        if (result == null)
            return NotFound("Unable to fetch location");

        return Ok(new
        {
            ip = result.Query,
            city = result.City,
            region = result.RegionName,
            country = result.Country,
            latitude = result.Lat,
            longitude = result.Lon,
            isp = result.Isp
        });
    }
}


