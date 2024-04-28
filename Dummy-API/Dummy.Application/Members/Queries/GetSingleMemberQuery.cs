using Dummy.Application.Locations.DTOs;
using Dummy.Application.Members.DTOs;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Dummy.Application.Members.Queries
{
    public record GetSingleMemberQuery(String Slug) : IRequestWithBaseResponse<MemberDTO>;
    internal class GetSingleMemberQueryHandler : IRequestWithBaseResponseHandler<GetSingleMemberQuery, MemberDTO>
    {
        private readonly MainDBContext _mainDBContext;

        public GetSingleMemberQueryHandler(MainDBContext mainDBContext)
        {
            _mainDBContext = mainDBContext;
        }
        public async Task<BaseResponseDTO<MemberDTO>> Handle(GetSingleMemberQuery request, CancellationToken cancellationToken)
        {
            var query = _mainDBContext.Members.AsNoTracking()
                                               .Select(x => new MemberDTO
                                               {
                                                   Slug = x.Slug,
                                                   FirstName = x.FirstName,
                                                   LastName = x.LastName,
                                                   Email = x.Email,
                                                   Position = x.Position,
                                                   Avatar = x.Avatar,
                                                   CreatedDate = x.CreatedDate,
                                                   UpdatedDate = x.UpdatedDate,
                                                   Locations = x.Locations.Select(l => new LocationDTO
                                                   {
                                                       Address = l.Address,
                                                       District = l.District,
                                                       City = l.City
                                                   })
                                               });

            var result = await query.FirstOrDefaultAsync(x => x.Slug == request.Slug, cancellationToken);

            if (result is null)
            {
                throw new RestfulAPIException(HttpStatusCode.NotFound, $"{request.Slug} member does not exists!");
            }

            // Map to DTO
            var memberDTO = new MemberDTO
            {
                Slug = result.Slug,
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                Position = result.Position,
                Avatar = result.Avatar,
                Locations = result.Locations,
                CreatedDate = result.CreatedDate,
                UpdatedDate = result.UpdatedDate,
            };

            return new BaseResponseDTO<MemberDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully retrieve {result.Slug} member",
                Data = memberDTO
            };
        }
    }
}
