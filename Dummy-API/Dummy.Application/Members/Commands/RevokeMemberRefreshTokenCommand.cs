using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Services.Auth;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public record RevokeMemberRefreshTokenCommand(String Slug) : IRequestWithBaseResponse;

    // Command validation
    public class RevokeMemberRefreshTokenCommandValidator : AbstractValidator<RevokeMemberRefreshTokenCommand>
    {
        private readonly IStringLocalizer<RevokeMemberRefreshTokenCommandValidator> _localizer;

        public RevokeMemberRefreshTokenCommandValidator(IStringLocalizer<RevokeMemberRefreshTokenCommandValidator> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Slug).NotEmpty()
                                .OverridePropertyName(_localizer["slug"])
                                .WithMessage(_localizer["failure.cant_be_empty"]);
        }
    }
    internal class RevokeMemberRefreshTokenCommandHandler : IRequestWithBaseResponseHandler<RevokeMemberRefreshTokenCommand>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<RevokeMemberRefreshTokenCommandHandler> _localizer;

        public RevokeMemberRefreshTokenCommandHandler(MainDBContext mainDBContext,
                                                      IStringLocalizer<RevokeMemberRefreshTokenCommandHandler> localizer,
                                                      ICurrentUserService currentUserService)
        {
            _localizer = localizer;
            _mainDBContext = mainDBContext;
            _currentUserService = currentUserService;
        }
        public async Task<BaseResponseDTO> Handle(RevokeMemberRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshTokenLists = await _mainDBContext.RefreshTokens.Include(x => x.Member)
                                                                      .Where(x => x.MemberId == _currentUserService.Id && x.Member.Slug == request.Slug)
                                                                      .ToListAsync(cancellationToken);

            _mainDBContext.RefreshTokens.RemoveRange(refreshTokenLists);
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO
            {
                Code = HttpStatusCode.NoContent,
                Message = _localizer["successful.revoke_token"]
            };
        }
    }
}
