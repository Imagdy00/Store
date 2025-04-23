using Domain.Exceptions;
using Shared.ErrorModels;

namespace Store.Api.Middlewares;

public class GolobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GolobalErrorHandlingMiddleware> _logger;

    public GolobalErrorHandlingMiddleware(RequestDelegate next , ILogger<GolobalErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
            if(context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                await HandlingNotFoundEndPointAsync(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandlingErrorAsync(context, ex);

        }
    }

    private static async Task HandlingErrorAsync(HttpContext context, Exception ex)
    {
        //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new ErrorDetails()
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            ErrorMessage = ex.Message
        };


        response.StatusCode = ex switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            BadRequestException => StatusCodes.Status400BadRequest,
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => HandleValidationExceptionAsync((ValidationException)ex , response),
            _ => StatusCodes.Status500InternalServerError
        };


        context.Response.StatusCode = response.StatusCode;


        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task HandlingNotFoundEndPointAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        var response = new ErrorDetails()
        {
            StatusCode = StatusCodes.Status404NotFound,
            ErrorMessage = $"End Point {context.Request.Path} is not found"
        };


        await context.Response.WriteAsJsonAsync(response);
    }

    private static  int HandleValidationExceptionAsync(ValidationException ex , ErrorDetails response)
    {
        response.Errors = ex.Errors;
        return StatusCodes.Status400BadRequest;

    }


}
