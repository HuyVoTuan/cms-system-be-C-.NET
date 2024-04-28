using Dummy.Domain.Constants;
using Dummy.Domain.Entities;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Helpers;
using Dummy.Infrastructure.Services.Auth;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public class RegisterMemberCommand() : IRequestWithBaseResponse<AuthResponseDTO>
    {
        public String Avatar { get; init; }
        public String FirstName { get; init; }
        public String LastName { get; init; }
        public String Password { get; set; }
        public String Email { get; init; }
        public String Address { get; init; }
        public String District { get; init; }
        public String City { get; init; }
    }

    internal class RegisterMemberCommandHandler : IRequestWithBaseResponseHandler<RegisterMemberCommand, AuthResponseDTO>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly IAuthService _authService;

        public RegisterMemberCommandHandler(MainDBContext mainDBContext, IAuthService authService)
        {
            _mainDBContext = mainDBContext;
            _authService = authService;
        }
        public async Task<BaseResponseDTO<AuthResponseDTO>> Handle(RegisterMemberCommand request, CancellationToken cancellationToken)
        {
            var isMemeberExist = await _mainDBContext.Members.AsNoTracking()
                                                             .AnyAsync(x => x.Email == request.Email);

            if (isMemeberExist)
            {
                throw new RestfulAPIException(HttpStatusCode.Found, $"User with {request.Email} is already exists!");
            }

            var newMember = new Member
            {
                Slug = StringHelper.GenerateSlug($"{request.FirstName} {request.LastName}"),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = _authService.HashPassword(request.Password),
                Email = request.Email,
                Avatar = request.Avatar,
                Position = MemberRole.NewlyCreated,
            };

            var newMemberLocation = new Location
            {
                Member = newMember,
                Address = request.Address,
                City = request.City,
                District = request.District,
            };

            // Sequentially add user and their location to database
            await _mainDBContext.Members.AddAsync(newMember, cancellationToken);
            await _mainDBContext.Locations.AddAsync(newMemberLocation, cancellationToken);

            // Initial save to database
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            var refreshToken = _authService.GenerateRefreshToken(newMember);
            await _mainDBContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

            // Second save to database
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var authResponseDTO = new AuthResponseDTO
            {
                AccessToken = _authService.GenerateToken(newMember),
                RefreshToken = refreshToken.Token
            };

            return new BaseResponseDTO<AuthResponseDTO>
            {
                Code = HttpStatusCode.Created,
                Message = $"Successfully register {request.Email} user",
                Data = authResponseDTO
            };
        }
    }
}
