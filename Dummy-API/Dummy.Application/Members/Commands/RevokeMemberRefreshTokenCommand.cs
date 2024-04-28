using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Services.Auth;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public record RevokeMemberRefreshTokenCommand(String Slug) : IRequestWithBaseResponse;
    internal class RevokeMemberRefreshTokenCommandHandler : IRequestWithBaseResponseHandler<RevokeMemberRefreshTokenCommand>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly ICurrentUserService _currentUserService;

        public RevokeMemberRefreshTokenCommandHandler(MainDBContext mainDBContext, ICurrentUserService currentUserService)
        {
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
                Message = "Successfully revoke all current user tokens"
            };
        }
    }
}
