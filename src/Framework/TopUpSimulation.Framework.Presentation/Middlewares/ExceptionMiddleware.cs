using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TopUpSimulation.Framework.Common.Exceptions;

namespace TopUpSimulation.Framework.Presentation.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        Stream body = httpContext.Response.Body;
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            httpContext.Response.Body = body;
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    public virtual int ExceptionStatusCodeMapping(Exception ex)
    {
        var num = ex switch
        {
            InvalidCastException _ => 400,
            InvalidOperationException _ => 201,
            ValidationException _ => 401,
            TopUpException _ => 200,
            _ => 500,
        };
        return num;
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int status = ExceptionStatusCodeMapping(exception);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;
        if (status >= 500)
        {
            _logger.LogError(exception, "خطای سرور رخ داده است");
        }
        else
            _logger.LogInformation(exception, "Handled exception occurred");
    }
}