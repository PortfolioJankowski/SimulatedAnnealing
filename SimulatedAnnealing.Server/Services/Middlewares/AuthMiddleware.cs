using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Server.Services.Middlewares;

public class AuthMiddleware : IMiddleware
{
    private readonly ILogger<AuthMiddleware> _logger;

    public AuthMiddleware(ILogger<AuthMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation($"Processing request: {context.Request.Path}");
        if (context.Request.Path.StartsWithSegments("/api/Account")){ await next(context); return; }

        if (!context.User.Identity.IsAuthenticated)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized; 
            await context.Response.WriteAsync("Unauthorized: Authentication required.");
            return;
        }

        context.Response.Headers.Add("Zapytaj", "Beczke");
        await next(context);
    }
}


