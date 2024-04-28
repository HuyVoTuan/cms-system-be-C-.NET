using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Services.Auth;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public class LoginMemberCommand() : IRequestWithBaseResponse<AuthResponseDTO>
    {
        public String Email { get; init; }
        public String Password { get; init; }

    }
    internal class LoginMemberCommandHandler : IRequestWithBaseResponseHandler<LoginMemberCommand, AuthResponseDTO>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly IAuthService _authService;

        public LoginMemberCommandHandler(MainDBContext mainDBContext, IAuthService authService)
        {
            _mainDBContext = mainDBContext;
            _authService = authService;
        }
        public async Task<BaseResponseDTO<AuthResponseDTO>> Handle(LoginMemberCommand request, CancellationToken cancellationToken)
        {
            var existedMember = await _mainDBContext.Members.AsNoTracking()
                                                            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);


            if (existedMember is null || !_authService.VerifyPassword(request.Password, existedMember.Password))
            {
                throw new RestfulAPIException(HttpStatusCode.NotFound, $"User {request.Email} does not exists!");
            }


            var newRefreshToken = _authService.GenerateRefreshToken(existedMember);
            await _mainDBContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);

            // Save to database
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var authResponseDTO = new AuthResponseDTO
            {
                AccessToken = _authService.GenerateToken(existedMember),
                RefreshToken = newRefreshToken.Token
            };

            return new BaseResponseDTO<AuthResponseDTO>
            {
                Code = HttpStatusCode.OK,
                Message = "Successfully login in",
                Data = authResponseDTO
            };
        }
    }
}
