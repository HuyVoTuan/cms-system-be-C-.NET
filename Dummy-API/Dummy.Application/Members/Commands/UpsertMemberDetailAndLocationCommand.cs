using Dummy.Application.Locations.DTOs;
using Dummy.Application.Members.DTOs;
using Dummy.Domain.Entities;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Helpers;
using Dummy.Infrastructure.Services.Auth;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public class UpsertMemberDetailAndLocationCommandRequestDTO : IRequestWithBaseResponse<MemberDTO>
    {
        public String Avatar { get; init; }
        public String FirstName { get; init; }
        public String LastName { get; init; }
        public String Email { get; init; }
        public String Position { get; init; }
        public String Address { get; init; }
        public String District { get; init; }
        public String City { get; init; }
    }

    public class UpsertMemberDetailAndLocationCommandRequestDTOValidator : AbstractValidator<UpsertMemberDetailAndLocationCommandRequestDTO>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly IStringLocalizer<UpsertMemberDetailAndLocationCommandRequestDTOValidator> _localizer;

        public UpsertMemberDetailAndLocationCommandRequestDTOValidator(MainDBContext mainDBContext, IStringLocalizer<UpsertMemberDetailAndLocationCommandRequestDTOValidator> localizer)
        {
            _localizer = localizer;
            _mainDBContext = mainDBContext;

            RuleFor(x => x.FirstName).NotEmpty()
                                     .OverridePropertyName(_localizer["firstname"])
                                     .WithMessage(_localizer["failure.cant_be_empty"]);

            RuleFor(x => x.LastName).NotEmpty()
                                    .OverridePropertyName(_localizer["lastname"])
                                    .WithMessage(_localizer["failure.cant_be_empty"]);

            RuleFor(x => x.Email).EmailAddress()
                                 .OverridePropertyName(_localizer["email"])
                                 .WithMessage(_localizer["failure.invalid"])
                                 .NotEmpty()
                                 .OverridePropertyName(_localizer["email"])
                                 .WithMessage(_localizer["failure.cant_be_empty"])
                                 .Must(email =>
                                 {
                                     var isAdminEmail = email.Contains("admin");
                                     return !isAdminEmail;
                                 })
                                 .OverridePropertyName(_localizer["email"])
                                 .WithMessage(_localizer["failure.invalid"])
                                 .Must(email =>
                                 {
                                     var isExisted = _mainDBContext.Members.Any(x => x.Email == email);
                                     return !isExisted;
                                 })
                                .OverridePropertyName(_localizer["email"])
                                .WithMessage(_localizer["failure.already_exists"]);

            RuleFor(x => x.Position).NotEmpty()
                                    .OverridePropertyName(_localizer["position"])
                                    .WithMessage(_localizer["failure.cant_be_empty"]);

            RuleFor(x => x.Address).NotEmpty()
                                   .OverridePropertyName(_localizer["address"])
                                   .WithMessage(_localizer["failure.cant_be_empty"]);

            RuleFor(x => x.District).NotEmpty()
                                    .OverridePropertyName(_localizer["district"])
                                    .WithMessage(_localizer["failure.cant_be_empty"]);

            RuleFor(x => x.City).NotEmpty()
                                .OverridePropertyName(_localizer["city"])
                                .WithMessage(_localizer["failure.cant_be_empty"]);
        }
    }
    public class UpsertMemberDetailAndLocationCommand : IRequestWithBaseResponse<MemberDTO>
    {
        public String Slug { get; init; }
        public String Avatar { get; init; }
        public String FirstName { get; init; }
        public String LastName { get; init; }
        public String Email { get; init; }
        public String Position { get; init; }
        public String Address { get; init; }
        public String District { get; init; }
        public String City { get; init; }
    }

    internal class UpsertMemberDetailAndLocationCommandHandler : IRequestWithBaseResponseHandler<UpsertMemberDetailAndLocationCommand, MemberDTO>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<UpsertMemberDetailAndLocationCommandHandler> _localizer;

        public UpsertMemberDetailAndLocationCommandHandler(MainDBContext mainDBContext,
                                                           IStringLocalizer<UpsertMemberDetailAndLocationCommandHandler> localizer,
                                                           ICurrentUserService currentUserService)
        {
            _localizer = localizer;
            _mainDBContext = mainDBContext;
            _currentUserService = currentUserService;
        }
        public async Task<BaseResponseDTO<MemberDTO>> Handle(UpsertMemberDetailAndLocationCommand request, CancellationToken cancellationToken)
        {
            var isAdmin = StringHelper.GenerateSlug($"{request.FirstName} {request.LastName}").Contains("admin");
            var existingMember = await _mainDBContext.Members.Include(x => x.Locations)
                                                             .FirstOrDefaultAsync(x => x.Slug == request.Slug
                                                                                  && x.Id == _currentUserService.Id, cancellationToken);

            if (existingMember is null)
            {
                throw new RestfulAPIException(HttpStatusCode.NotFound, $"{request.Slug} {_localizer["failure.not_exists"]}");
            }

            if (isAdmin)
            {
                throw new RestfulAPIException(HttpStatusCode.BadRequest, _localizer["failure.invalid"]);
            }

            existingMember.Slug = StringHelper.GenerateSlug($"{request.FirstName} {request.LastName}");
            existingMember.FirstName = request.FirstName;
            existingMember.LastName = request.LastName;
            existingMember.Email = request.Email;
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
            var memberDTO = new MemberDTO
            {
                Slug = existingMember.Slug,
                FirstName = existingMember.FirstName,
                LastName = existingMember.LastName,
                Email = existingMember.Email,
                Position = existingMember.Position,
                Avatar = existingMember.Avatar,
                Locations = existingMember.Locations.Select(x => new LocationDTO
                {
                    Address = x.Address,
                    District = x.District,
                    City = x.City,
                }),
            };

            // Save to database
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO<MemberDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"{_localizer["successful.update"]} {existingMember.Slug}",
                Data = memberDTO
            };
        }
    }
}
