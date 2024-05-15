using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Extensions;
using Dummy.Infrastructure.Services.Auth;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public record RevokeCurrentMemberRefreshTokenCommand() : IRequestWithBaseResponse;

    internal class RevokeCurrentMemberRefreshTokenCommandHandler : IRequestWithBaseResponseHandler<RevokeCurrentMemberRefreshTokenCommand>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<RevokeCurrentMemberRefreshTokenCommandHandler> _localizer;

        public RevokeCurrentMemberRefreshTokenCommandHandler(MainDBContext mainDBContext,
        IStringLocalizer<RevokeCurrentMemberRefreshTokenCommandHandler> localizer, ICurrentUserService currentUserService)
        {
            _localizer = localizer;
            _mainDBContext = mainDBContext;
            _currentUserService = currentUserService;
        }
        public async Task<BaseResponseDTO> Handle(RevokeCurrentMemberRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshTokenLists = await _mainDBContext.RefreshTokens.Include(x => x.Member)
                                                                      .Where(x => x.MemberId == _currentUserService.Id)
                                                                      .ToListAsync(cancellationToken);

            _mainDBContext.RefreshTokens.RemoveRange(refreshTokenLists);
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO
            {
                Code = HttpStatusCode.NoContent,
                Message = _localizer.Translate("successful.revoke_token", new List<String> { "user" })
            };
        }
    }
}
