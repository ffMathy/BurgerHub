using MediatR;
using Serilog.Context;
using ILogger = Serilog.ILogger;

namespace BurgerHub.Api.Infrastructure.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger _logger;

    public LoggingBehavior(
        ILogger logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        using (LogContext.PushProperty("RequestObject", request, true))
        {
            _logger.Verbose("Handling {RequestName}", typeof(TRequest).Name);
            
            var response = await next();
            
            _logger.Verbose("Handled {RequestName}", typeof(TRequest).Name);

            return response;
        }
    }
}