using Dummy.Infrastructure.Commons.Base;
using FluentValidation;
using MediatR;
using System.Net;

namespace Dummy.Infrastructure.Behaviors
{
    public class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults.SelectMany(result => result.Errors)
                                            .Where(failure => failure != null)
                                            .GroupBy(message => message.PropertyName)
                                            .ToList()
                                            .ToDictionary(message => message.Key, message => message.Select(content => content.ErrorMessage.Replace("'", "")));

            if (failures.Any())
            {
                throw new RestfulAPIException(HttpStatusCode.BadRequest, failures);
            }

            return await next();
        }
    }
}
