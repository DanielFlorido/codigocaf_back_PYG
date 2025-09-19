using CodigoCAFBack.Aplicacion.Excepciones;

namespace CodigoCAFBack.API.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex.Message);
            await HandleException(context, ex);
        }
    }

    private static Task HandleException(HttpContext context, Exception ex)
    {
        int statusCode = StatusCodes.Status500InternalServerError;
        string mesagge = "Ha ocurrido un error, Contactese con el aministrador";
        switch (ex)
        {
            case NoEncontradoExcepcion _:
                statusCode = StatusCodes.Status404NotFound;
                break;
            case CodigoCAFExcepcion _:
                statusCode = StatusCodes.Status400BadRequest;
                mesagge = ex.Message;
                break;
            case DivideByZeroException _:
                statusCode = StatusCodes.Status400BadRequest;
                break;
        }
        MensajeExcepcion mensajeExcepcion = new MensajeExcepcion() { Mensaje = mesagge };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(mensajeExcepcion.ToString());
    }
}

// Extension method for this middleware

public static class ExceptionMiddlewareExtension
{
    public static void CongigureExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
