using System.Diagnostics;
using System.Text;

namespace RealEstate.API.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Log request
        await LogRequest(context);

        // Capture original response body stream
        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Execute the next middleware
            await _next(context);

            // Log response
            stopwatch.Stop();
            await LogResponse(context, stopwatch.ElapsedMilliseconds);

            // Copy response back to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();

        var requestBody = string.Empty;
        if (context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        _logger.LogInformation(
            "HTTP {Method} {Path} {QueryString} - Request Body: {RequestBody}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            string.IsNullOrEmpty(requestBody) ? "Empty" : requestBody);
    }

    private async Task LogResponse(HttpContext context, long elapsedMs)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation(
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms - Response: {ResponseBody}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsedMs,
            string.IsNullOrEmpty(responseBody) ? "Empty" : responseBody);
    }
}
