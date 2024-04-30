using Dummy.Domain.Constants;
using Dummy.Domain.Entities;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Helpers;
using Dummy.Infrastructure.Services.Auth;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public class RegisterMemberCommand() : IRequestWithBaseResponse<AuthResponseDTO>
    {
        public String Avatar { get; init; }
        public String FirstName { get; init; }
        public String LastName { get; init; }
        public String Password { get; init; }
        public String Email { get; init; }
        public String Address { get; init; }
        public String District { get; init; }
        public String City { get; init; }
    }

    // Command validation
    public class RegisterMemberCommandValidator : AbstractValidator<RegisterMemberCommand>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly IStringLocalizer<RegisterMemberCommandValidator> _localizer;

        public RegisterMemberCommandValidator(MainDBContext mainDBContext, IStringLocalizer<RegisterMemberCommandValidator> localizer)
        {
            _localizer = localizer;
            _mainDBContext = mainDBContext;


            RuleFor(x => x.FirstName).NotEmpty()
                                     .OverridePropertyName(_localizer["Firstname"])
                                     .WithMessage(_localizer["Firstname can not be empty!"]);

            RuleFor(x => x.LastName).NotEmpty()
                                    .OverridePropertyName("Lastname")
                                    .WithMessage(_localizer["Lastname can not be empty!"]);

            RuleFor(x => x.Email).Must(email =>
                                 {
                                     var isExisted = _mainDBContext.Members.Any(x => x.Email == email);
                                     return !isExisted;
                                 })
                                .OverridePropertyName("Email")
                                .WithMessage("Email has been taken!");

            RuleFor(x => x.Password).NotEmpty()
                                    .MinimumLength(6)
                                    .OverridePropertyName("password")
                                    .WithMessage("Password can not be empty or less than 6 characters!");

            RuleFor(x => x.Address).NotEmpty()
                                    .OverridePropertyName("address")
                                    .WithMessage("Address can not be empty!");

            RuleFor(x => x.District).NotEmpty()
                                    .OverridePropertyName("district")
                                    .WithMessage("District can not be empty!");

            RuleFor(x => x.City).NotEmpty()
                                .OverridePropertyName("city")
                                .WithMessage("City can not be empty!");
        }
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
            var isAdmin = StringHelper.GenerateSlug($"{request.FirstName} {request.LastName}").Contains("admin");

            if (isAdmin)
            {
                throw new RestfulAPIException(HttpStatusCode.BadRequest, "Invalid username, please choose another username!");
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
                Message = $"Successfully register {newMember.Slug} user",
                Data = authResponseDTO
            };
        }
    }
}
