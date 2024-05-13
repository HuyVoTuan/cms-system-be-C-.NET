using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Dummy.Infrastructure.Behaviors
{
    internal class MediatRLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<MediatRLoggingBehavior<TRequest, TResponse>> _logger;

        public MediatRLoggingBehavior(ILogger<MediatRLoggingBehavior<TRequest, TResponse>> logger)
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

            _logger.LogInformation($"[START] Handling {requestNameWithGuid}");

            // Calculate execution time for the request
            TResponse response;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                try
                {
                    _logger.LogInformation($"[PROPS] {requestNameWithGuid} {JsonConvert.SerializeObject(request)}");
                }
                catch (NotSupportedException)
                {
                    _logger.LogError($"[Serialization ERROR] {requestNameWithGuid} Could not serialize the request");
                }

                response = await next();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    $"[END] {requestNameWithGuid}; Execution time={stopwatch.ElapsedMilliseconds}ms");
            }

            _logger.LogInformation($"Handled {typeof(TResponse).Name}");

            return response;
        }
    }
}
