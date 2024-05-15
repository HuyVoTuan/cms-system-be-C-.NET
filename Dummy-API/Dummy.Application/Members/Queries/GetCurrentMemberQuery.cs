using Dummy.Application.Locations.DTOs;
using Dummy.Application.Members.DTOs;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dummy.Application.Members.Queries
{
    public record GetCurrentMemberQuery() : IRequestWithBaseResponse<MemberDTO>;
    internal class GetCurrentMemberQueryHandler : IRequestWithBaseResponseHandler<GetCurrentMemberQuery, MemberDTO>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<GetCurrentMemberQueryHandler> _localizer;

        public GetCurrentMemberQueryHandler(MainDBContext mainDBContext, ICurrentUserService currentUserService, IStringLocalizer<GetCurrentMemberQueryHandler> localizer)
        {
            _localizer = localizer;
            _mainDBContext = mainDBContext;
            _currentUserService = currentUserService;
        }
        public async Task<BaseResponseDTO<MemberDTO>> Handle(GetCurrentMemberQuery request, CancellationToken cancellationToken)
        {
            var currentMember = await _mainDBContext.Members.AsNoTracking()
                                                            .Include(x => x.Locations)
                                                            .FirstOrDefaultAsync(x => x.Id == _currentUserService.Id, cancellationToken);

            var currentMemberDTO = new MemberDTO
            {
                Slug = currentMember.Slug,
                FirstName = currentMember.FirstName,
                LastName = currentMember.LastName,
                Email = currentMember.Email,
                Position = currentMember.Position,
                Avatar = currentMember.Avatar,
                CreatedDate = currentMember.CreatedDate,
                UpdatedDate = currentMember.UpdatedDate,
                Locations = currentMember.Locations.Select(l => new LocationDTO
                {
                    Address = l.Address,
                    District = l.District,
                    City = l.City
                })
            };

            return new BaseResponseDTO<MemberDTO>
            {
                Code = HttpStatusCode.OK,
                Message = _localizer["successful.retrieve", _localizer["user"]],
                Data = currentMemberDTO
            };
        }
    }
}
