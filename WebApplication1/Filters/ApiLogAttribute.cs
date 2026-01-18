using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication1.Models;

public class ApiLogAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var http = context.HttpContext;

        // Method & Path
        var method = http.Request.Method;
        var path = http.Request.Path.ToString();

        // ✅ REAL CLIENT IP (proxy safe)
        string ip = http.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrEmpty(ip))
        {
            ip = ip.Split(',')[0].Trim(); // first IP is real client
        }
        else
        {
            ip = http.Connection.RemoteIpAddress?.ToString();
        }

        // UserId from JWT
        Guid? userId = null;
        var userIdClaim = http.User.FindFirst("UserId")?.Value;

        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out Guid uid))
        {
            userId = uid;
        }

        // Request Body
        string requestBody = JsonSerializer.Serialize(context.ActionArguments);

        // Execute API
        var resultContext = await next();

        // Response
        string responseBody = "";
        int statusCode = http.Response.StatusCode;

        if (resultContext.Result is ObjectResult objResult)
        {
            responseBody = JsonSerializer.Serialize(objResult.Value);
            statusCode = objResult.StatusCode ?? statusCode;
        }

        // DB Save
        using var scope = http.RequestServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var log = new ApiLog
        {
            UserId = userId,
            IpAddress = ip,
            Method = method,
            Path = path,
            RequestBody = requestBody,
            ResponseBody = responseBody,
            StatusCode = statusCode,
            CreatedAt = DateTime.UtcNow
        };

        await db.ApiLogs.AddAsync(log);
        await db.SaveChangesAsync();
    }
}
