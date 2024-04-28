using Dummy.Application.Locations.DTOs;
using Dummy.Application.Members.DTOs;
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

        public UpsertMemberDetailAndLocationCommandHandler(MainDBContext mainDBContext, ICurrentUserService currentUserService)
        {
            _mainDBContext = mainDBContext;
            _currentUserService = currentUserService;
        }
        public async Task<BaseResponseDTO<MemberDTO>> Handle(UpsertMemberDetailAndLocationCommand request, CancellationToken cancellationToken)
        {
            var existingMember = await _mainDBContext.Members.Include(x => x.Locations)
                                                             .FirstOrDefaultAsync(x => x.Slug == request.Slug
                                                                                  && x.Id == _currentUserService.Id, cancellationToken);

            if (existingMember is null)
            {
                throw new RestfulAPIException(HttpStatusCode.NotFound, $"{request.Slug} member does not exists!");
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
                CreatedDate = existingMember.CreatedDate,
                UpdatedDate = existingMember.UpdatedDate
            };

            // Save to database
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO<MemberDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully update {request.Email} user",
                Data = memberDTO
            };
        }
    }
}
