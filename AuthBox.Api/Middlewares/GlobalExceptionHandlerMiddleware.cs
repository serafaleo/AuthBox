using AuthBox.Models.Dtos;
using AuthBox.Models.Enums;
using AuthBox.Utils.ExtensionMethods;
using Microsoft.VisualBasic;
using System.Net;
using System.Text.Json;

namespace AuthBox.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            string message = ex.GetInnermostMessage();

            _logger.LogError(ex, message); // TODO(serafa.leo): Como ver esse log?
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            ResponseDto response = new()
            {
                Status = EResponseStatus.Exception,
                Object = message,
            };

            string json = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    }
}
