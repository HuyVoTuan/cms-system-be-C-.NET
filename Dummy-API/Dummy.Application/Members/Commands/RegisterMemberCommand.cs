using Dummy.Domain.Constants;
using Dummy.Domain.Entities;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Helpers;
using Dummy.Infrastructure.Services.Auth;
using Dummy.Infrastructure.Services.EmailService;
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
                                     .OverridePropertyName(_localizer["firstname"])
                                     .WithMessage(_localizer["cant_be_empty"]);

            RuleFor(x => x.LastName).NotEmpty()
                                    .OverridePropertyName(_localizer["lastname"])
                                    .WithMessage(_localizer["cant_be_empty"]);

            RuleFor(x => x.Email).EmailAddress()
                                 .OverridePropertyName(_localizer["email"])
                                 .WithMessage(_localizer["invalid"])
                                 .NotEmpty()
                                 .OverridePropertyName(_localizer["email"])
                                 .WithMessage(_localizer["cant_be_empty"])
                                 .Must(email =>
                                 {
                                     var isExisted = _mainDBContext.Members.Any(x => x.Email == email);
                                     return !isExisted;
                                 })
                                .OverridePropertyName(_localizer["email"])
                                .WithMessage(_localizer["already_exists"]);

            RuleFor(x => x.Password).NotEmpty()
                                    .OverridePropertyName(_localizer["password.password"])
                                    .WithMessage(_localizer["cant_be_empty"])
                                    .MinimumLength(6)
                                    .OverridePropertyName(_localizer["password.password"])
                                    .WithMessage(_localizer["password.min_6_length"]);

            RuleFor(x => x.Address).NotEmpty()
                                   .OverridePropertyName(_localizer["address"])
                                   .WithMessage(_localizer["cant_be_empty"]);

            RuleFor(x => x.District).NotEmpty()
                                    .OverridePropertyName(_localizer["district"])
                                    .WithMessage(_localizer["cant_be_empty"]);

            RuleFor(x => x.City).NotEmpty()
                                .OverridePropertyName(_localizer["city"])
                                .WithMessage(_localizer["cant_be_empty"]);
        }
    }

    internal class RegisterMemberCommandHandler : IRequestWithBaseResponseHandler<RegisterMemberCommand, AuthResponseDTO>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly IAuthService _authService;
        private readonly IEmailNotificationService _emailNotificationService;

        public RegisterMemberCommandHandler(MainDBContext mainDBContext, IAuthService authService, IEmailNotificationService emailNotificationService)
        {
            _mainDBContext = mainDBContext;
            _authService = authService;
            _emailNotificationService = emailNotificationService;
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

            await _emailNotificationService.SendEmailAsync("huy.vt00578@sinhvien.hoasen.edu.vn", new {Username = newMember.Slug }, "register");

            return new BaseResponseDTO<AuthResponseDTO>
            {
                Code = HttpStatusCode.Created,
                Message = $"Successfully register {newMember.Slug} user",
                Data = authResponseDTO
            };
        }
    }
}
