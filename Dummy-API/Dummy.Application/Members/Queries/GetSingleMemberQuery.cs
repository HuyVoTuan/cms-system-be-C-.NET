using Dummy.Application.Locations.DTOs;
using Dummy.Application.Members.DTOs;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Queries
{
    public record GetSingleMemberQuery(String Slug) : IRequestWithBaseResponse<MemberDTO>;
    internal class GetSingleMemberQueryHandler : IRequestWithBaseResponseHandler<GetSingleMemberQuery, MemberDTO>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly IStringLocalizer<GetSingleMemberQueryHandler> _localizer;

        public GetSingleMemberQueryHandler(MainDBContext mainDBContext, IStringLocalizer<GetSingleMemberQueryHandler> localizer)
        {
            _localizer = localizer;
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

            var memberDTO = await query.FirstOrDefaultAsync(x => x.Slug == request.Slug, cancellationToken);

            if (memberDTO is null)
            {
                throw new RestfulAPIException(HttpStatusCode.NotFound, _localizer["failure.not_exists", _localizer["user"]]);
            }

            return new BaseResponseDTO<MemberDTO>
            {
                Code = HttpStatusCode.OK,
                Message = _localizer["successful.retrieve", _localizer["user"]],
                Data = memberDTO
            };
        }
    }
}
