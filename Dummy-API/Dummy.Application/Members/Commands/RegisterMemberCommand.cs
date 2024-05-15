using Dummy.Domain.Entities;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Extensions;
using Dummy.Infrastructure.Helpers;
using Dummy.Infrastructure.Services.Auth;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public class RegisterMemberCommand : IRequestWithBaseResponse<AuthResponseDTO>
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
                                     .OverridePropertyName(_localizer.Translate("firstname"))
                                     .WithMessage(_localizer.Translate("failure.cant_be_empty"))
                                     .Must(StringHelper.IsValidString)
                                     .OverridePropertyName(_localizer.Translate("firstname"))
                                     .WithMessage(_localizer.Translate("failure.invalid"));

            RuleFor(x => x.LastName).NotEmpty()
                                    .OverridePropertyName(_localizer.Translate("lastname"))
                                    .WithMessage(_localizer.Translate("failure.cant_be_empty"))
                                    .Must(StringHelper.IsValidString)
                                    .OverridePropertyName(_localizer.Translate("lastname"))
                                    .WithMessage(_localizer.Translate("failure.invalid"));

            RuleFor(x => x.Email).EmailAddress()
                                 .OverridePropertyName(_localizer.Translate("email"))
                                 .WithMessage(_localizer.Translate("failure.invalid"))
                                 .NotEmpty()
                                 .OverridePropertyName(_localizer.Translate("email"))
                                 .WithMessage(_localizer.Translate("failure.cant_be_empty"))
                                 .Must(email =>
                                 {
                                     var isAdminEmail = email.Contains("admin");
                                     return !isAdminEmail;
                                 })
                                 .OverridePropertyName(_localizer.Translate("email"))
                                 .WithMessage(_localizer.Translate("failure.invalid"))
                                 .Must(email =>
                                 {
                                     var isExisted = _mainDBContext.Members.Any(x => x.Email == email);
                                     return !isExisted;
                                 })
                                .OverridePropertyName(_localizer.Translate("email"))
                                .WithMessage(_localizer.Translate("failure.already_exists"));

            RuleFor(x => x.Password).NotEmpty()
                                    .OverridePropertyName(_localizer.Translate("password"))
                                    .WithMessage(_localizer.Translate("failure.cant_be_empty"))
                                    .MinimumLength(6)
                                    .OverridePropertyName(_localizer.Translate("password"))
                                    .WithMessage(_localizer.Translate("validation_rules.min_6_length"));

            RuleFor(x => x.Address).NotEmpty()
                                   .OverridePropertyName(_localizer.Translate("address"))
                                   .WithMessage(_localizer.Translate("failure.cant_be_empty"));

            RuleFor(x => x.District).NotEmpty()
                                    .OverridePropertyName(_localizer.Translate("district"))
                                    .WithMessage(_localizer.Translate("failure.cant_be_empty"));

            RuleFor(x => x.City).NotEmpty()
                                .OverridePropertyName(_localizer.Translate("city"))
                                .WithMessage(_localizer.Translate("failure.cant_be_empty"))
                                .Must(StringHelper.IsValidString)
                                .OverridePropertyName(_localizer.Translate("city"))
                                .WithMessage(_localizer.Translate("failure.invalid"));


        }
    }

    internal class RegisterMemberCommandHandler : IRequestWithBaseResponseHandler<RegisterMemberCommand, AuthResponseDTO>
    {
        private readonly IAuthService _authService;
        private readonly MainDBContext _mainDBContext;
        private readonly IStringLocalizer<RegisterMemberCommandHandler> _localizer;

        public RegisterMemberCommandHandler(MainDBContext mainDBContext,
        IAuthService authService, IStringLocalizer<RegisterMemberCommandHandler> localizer)
        {
            _localizer = localizer;
            _authService = authService;
            _mainDBContext = mainDBContext;
        }
        public async Task<BaseResponseDTO<AuthResponseDTO>> Handle(RegisterMemberCommand request, CancellationToken cancellationToken)
        {
            var slug = StringHelper.GenerateSlug($"{request.FirstName} {request.LastName}");
            var isAdminSlug = slug.Contains("admin");

            var isMemberExists = await _mainDBContext.Members.AsNoTracking()
                                                             .AnyAsync(x => x.Slug == slug, cancellationToken);

            if (isAdminSlug)
            {
                throw new RestfulAPIException(HttpStatusCode.BadRequest, _localizer.Translate("failure.invalid", new List<String> { "user" }));
            }

            if (isMemberExists)
            {
                throw new RestfulAPIException(HttpStatusCode.Conflict, _localizer.Translate("failure.already_exists", new List<String> { "user" }));
            }

            var newMember = new Member
            {
                Slug = slug,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = _authService.HashPassword(request.Password),
                Email = request.Email,
                Avatar = request.Avatar,
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

            // Create Refresh Token for new member
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
                Message = _localizer.Translate("successful.register", new List<String> { "user" }),
                Data = authResponseDTO
            };
        }
    }
}
