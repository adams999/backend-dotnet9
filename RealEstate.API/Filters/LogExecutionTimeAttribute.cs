using Microsoft.AspNetCore.Mvc.Filters;

namespace RealEstate.API.Filters;

/// <summary>
/// Action filter to log execution time of actions
/// </summary>
public class LogExecutionTimeAttribute : ActionFilterAttribute
{
    private readonly ILogger<LogExecutionTimeAttribute> _logger;

    public LogExecutionTimeAttribute(ILogger<LogExecutionTimeAttribute> logger)
    {
        _logger = logger;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var result = await next();
        
        stopwatch.Stop();

        var controllerName = context.RouteData.Values["controller"];
        var actionName = context.RouteData.Values["action"];

        _logger.LogInformation(
            "Action {Controller}.{Action} executed in {ElapsedMs}ms",
            controllerName,
            actionName,
            stopwatch.ElapsedMilliseconds);
    }
}
