using Dummy.Application.Locations.DTOs;
using Dummy.Application.Members.DTOs;
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
    public class UpsertCurrentMemberDetailAndLocationCommand : IRequestWithBaseResponse<UpsertCurrentMemberDetailAndLocationDTO>
    {
        public String Avatar { get; init; }
        public String FirstName { get; init; }
        public String LastName { get; init; }
        public String Position { get; init; }
        public String Password { get; init; }
        public String Address { get; init; }
        public String District { get; init; }
        public String City { get; init; }
    }

    // Command Validator 
    public class UpsertCurrentMemberDetailAndLocationCommandValidator : AbstractValidator<UpsertCurrentMemberDetailAndLocationCommand>
    {
        private readonly IStringLocalizer<UpsertCurrentMemberDetailAndLocationCommandValidator> _localizer;

        public UpsertCurrentMemberDetailAndLocationCommandValidator(MainDBContext mainDBContext, IStringLocalizer<UpsertCurrentMemberDetailAndLocationCommandValidator> localizer)
        {
            _localizer = localizer;

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


    internal class UpsertCurrentMemberDetailAndLocationCommandHandler : IRequestWithBaseResponseHandler<UpsertCurrentMemberDetailAndLocationCommand, UpsertCurrentMemberDetailAndLocationDTO>
    {
        private readonly IAuthService _authService;
        private readonly MainDBContext _mainDBContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<UpsertCurrentMemberDetailAndLocationCommandHandler> _localizer;

        public UpsertCurrentMemberDetailAndLocationCommandHandler(MainDBContext mainDBContext, IAuthService authService,
        IStringLocalizer<UpsertCurrentMemberDetailAndLocationCommandHandler> localizer, ICurrentUserService currentUserService)
        {
            _localizer = localizer;
            _authService = authService;
            _mainDBContext = mainDBContext;
            _currentUserService = currentUserService;
        }
        public async Task<BaseResponseDTO<UpsertCurrentMemberDetailAndLocationDTO>> Handle(UpsertCurrentMemberDetailAndLocationCommand request, CancellationToken cancellationToken)
        {
            var slug = StringHelper.GenerateSlug($"{request.FirstName} {request.LastName}");
            var isAdminSlug = slug.Contains("admin");

            var existingMember = await _mainDBContext.Members.Include(x => x.Locations)
                                                             .FirstOrDefaultAsync(x => x.Id == _currentUserService.Id, cancellationToken);


            if (isAdminSlug)
            {
                throw new RestfulAPIException(HttpStatusCode.BadRequest, _localizer.Translate("failure.invalid", new List<String> { "user" }));
            }

            if (!StringHelper.IsSlugContainFullname(slug, existingMember.Slug))
            {
                existingMember.Slug = slug;
            }

            if (!_authService.VerifyPassword(request.Password, existingMember.Password))
            {
                existingMember.Password = _authService.HashPassword(request.Password);
            }

            existingMember.FirstName = request.FirstName;
            existingMember.LastName = request.LastName;
            existingMember.Avatar = request.Avatar;
            existingMember.Position = request.Position;

            // If the member already has a location, update its properties
            var existingMemeberLocation = existingMember.Locations.FirstOrDefault(
                    x => x.Address == request.Address
                    && x.District == request.District
                    && x.City == request.City
            );

            if (existingMemeberLocation is not null)
            {
                existingMemeberLocation.Address = request.Address;
                existingMemeberLocation.District = request.District;
                existingMemeberLocation.City = request.City;
            }
            else
            {
                var newExistingMemberLocation = new Location
                {
                    MemberId = existingMember.Id,
                    Address = request.Address,
                    City = request.City,
                    District = request.District,
                };

                await _mainDBContext.Locations.AddAsync(newExistingMemberLocation, cancellationToken);
            }

            // Map to DTO
            var memberDTO = new UpsertCurrentMemberDetailAndLocationDTO
            {
                Slug = existingMember.Slug,
                Avatar = existingMember.Avatar,
                FirstName = existingMember.FirstName,
                LastName = existingMember.LastName,
                Position = existingMember.Position,
                Locations = existingMember.Locations.Select(x => new LocationDTO
                {
                    Address = x.Address,
                    District = x.District,
                    City = x.City,
                }),
            };

            // Save to database
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO<UpsertCurrentMemberDetailAndLocationDTO>
            {
                Code = HttpStatusCode.OK,
                Message = _localizer.Translate("successful.update", new List<String> { "user" }),
                Data = memberDTO
            };
        }
    }
}
