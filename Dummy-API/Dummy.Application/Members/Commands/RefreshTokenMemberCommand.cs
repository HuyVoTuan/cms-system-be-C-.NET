using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Services;
using Dummy.Infrastructure.Services.Auth;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public class RefreshTokenMemberCommandDTO()
    {
        public string RefreshToken { get; init; }
    }

    public class RefreshTokenMemberCommand() : IRequestWithBaseResponse<AuthResponseDTO>
    {
        public String Slug { get; init; }
        public string RefreshToken { get; init; }
    }

    // Command validation
    public class RefreshTokenMemberCommandValidator : AbstractValidator<RefreshTokenMemberCommand>
    {
        private readonly IStringLocalizer _localizer;

        public RefreshTokenMemberCommandValidator(IStringLocalizer localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Slug).NotEmpty()
                                .OverridePropertyName(_localizer["slug"])
                                .WithMessage(_localizer["cant_be_empty"]);

            RuleFor(x => x.RefreshToken).NotEmpty()
                                        .OverridePropertyName(_localizer["refresh_token"])
                                        .WithMessage(_localizer["cant_be_empty"]);
        }
    }

    internal class RefreshTokenMemberCommandHandler : IRequestWithBaseResponseHandler<RefreshTokenMemberCommand, AuthResponseDTO>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICacheService _cacheService;

        public RefreshTokenMemberCommandHandler(MainDBContext mainDBContext,
                                                IAuthService authService,
                                                ICurrentUserService currentUserService,
                                                ICacheService cacheService)
        {
            _authService = authService;
            _cacheService = cacheService;
            _mainDBContext = mainDBContext;
            _currentUserService = currentUserService;
        }
        public async Task<BaseResponseDTO<AuthResponseDTO>> Handle(RefreshTokenMemberCommand request, CancellationToken cancellationToken)
        {
            var oldRefreshToken = await _mainDBContext.RefreshTokens
                                                      .Include(x => x.Member)
                                                      .FirstOrDefaultAsync(x => x.Token == request.RefreshToken
                                                                           && x.ExpiredDate > DateTime.UtcNow
                                                                           && x.MemberId == _currentUserService.Id,
                                                                           cancellationToken);
            // <summary>
            /* 
             * Check if oldRefreshToken in database exists? 
               1.) If not: check futhur in the cache memory mechanism
               2.) If oldRefreshToken is found in cache return that previous token else throw 401 status HTTP code
            */
            if (oldRefreshToken is null)
            {
                return CachedRefreshTokenHandler(request.RefreshToken);
            }


            // <summary>
            /* 
             * Check if oldRefreshToken in database exists? 
               1.) If yes: remove the oldRefreshToken in database with the current credential base on MemberId
               2.) Generate new access token and refresh token 
               3.) Cache in memory with the new {access token and refresh token } 
               4.) Save to database
               5.) Return response
            */
            _mainDBContext.RefreshTokens.Remove(oldRefreshToken);

            var newAccessToken = _authService.GenerateToken(oldRefreshToken.Member);
            var newRefreshToken = _authService.GenerateRefreshToken(oldRefreshToken.Member);

            _mainDBContext.RefreshTokens.Add(newRefreshToken);

            var authResponseDTO = new AuthResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };

            // Set the old refresh token with new authDTO response in the cache
            _cacheService.SetData<AuthResponseDTO>($"refreshTokenResponse-{oldRefreshToken.Token}", authResponseDTO, TimeSpan.FromSeconds(20));

            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO<AuthResponseDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully refresh user token",
                Data = authResponseDTO
            };
        }

        private BaseResponseDTO<AuthResponseDTO> CachedRefreshTokenHandler(string refreshToken)
        {
            var cachedRefreshToken = _cacheService.GetData<AuthResponseDTO>($"refreshTokenResponse-{refreshToken}");

            if (cachedRefreshToken is null)
            {
                throw new RestfulAPIException(HttpStatusCode.Unauthorized, "Invalid refresh token");
            }

            return new BaseResponseDTO<AuthResponseDTO>
            {
                Code = HttpStatusCode.OK,
                Message = "Successfully retrieve user refresh token",
                Data = cachedRefreshToken
            };
        }
    }
}
