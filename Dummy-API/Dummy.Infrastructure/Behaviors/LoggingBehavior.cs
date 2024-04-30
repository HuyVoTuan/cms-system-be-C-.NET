using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Dummy.Infrastructure.Behaviors
{
    internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        // Auto Logging mechanism through mediatR pipeline { Request and Response }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Generate unique id for each request type
            var requestName = request.GetType().Name;
            var requestGuid = Guid.NewGuid().ToString();

            var requestNameWithGuid = $"{requestName} [{requestGuid}]";

            _logger.LogInformation($"\n--[START] Handling {requestNameWithGuid}--\n");

            // Calculate execution time for the request
            TResponse response;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                try
                {
                    _logger.LogInformation($"\n--[PROPS] {requestNameWithGuid} {JsonConvert.SerializeObject(request)}--\n");
                }
                catch (NotSupportedException)
                {
                    _logger.LogError($"\n--[Serialization ERROR] {requestNameWithGuid} Could not serialize the request.--\n");
                }

                response = await next();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    $"\n--[END] {requestNameWithGuid}; Execution time={stopwatch.ElapsedMilliseconds}ms\n--");
            }

            _logger.LogInformation($"\n--Handled {typeof(TResponse).Name}--\n");

            return response;
        }
    }
}
