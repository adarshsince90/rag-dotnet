public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception occurred.");

            context.Response.StatusCode = 500;
            context.Response.ContentType =
                "application/json";

            var message =
                _environment.IsDevelopment()
                    ? ex.ToString()
                    : ex.Message;

            await context.Response.WriteAsJsonAsync(
                new ErrorResponse(
                    "InternalServerError",
                    message));
        }
    }
}