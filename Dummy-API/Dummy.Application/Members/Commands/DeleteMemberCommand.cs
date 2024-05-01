using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public record DeleteMemberCommand(String Slug) : IRequestWithBaseResponse;

    // Command validation
    public class DeleteMemberCommandValidator : AbstractValidator<DeleteMemberCommand>
    {
        private readonly IStringLocalizer _localizer;

        public DeleteMemberCommandValidator(IStringLocalizer localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Slug).NotEmpty()
                                .OverridePropertyName(_localizer["slug"])
                                .WithMessage(_localizer["cant_be_empty"]);
        }
    }
    internal class DeleteMemberCommandHandler : IRequestWithBaseResponseHandler<DeleteMemberCommand>
    {
        private readonly MainDBContext _mainDBContext;

        public DeleteMemberCommandHandler(MainDBContext mainDBContext)
        {
            _mainDBContext = mainDBContext;
        }
        public async Task<BaseResponseDTO> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
        {
            var existsMember = await _mainDBContext.Members.FirstOrDefaultAsync(x => x.Slug == request.Slug, cancellationToken);

            if (existsMember is null)
            {
                throw new RestfulAPIException(HttpStatusCode.NotFound, $"{request.Slug} member does not exists!");
            }

            _mainDBContext.Members.Remove(existsMember);
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO
            {
                Code = HttpStatusCode.NoContent,
                Message = $"Successfully delete ${existsMember.Slug} user"
            };
        }
    }
}
