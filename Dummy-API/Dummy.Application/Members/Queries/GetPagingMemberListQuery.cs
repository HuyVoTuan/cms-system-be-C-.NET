using Dummy.Application.Locations.DTOs;
using Dummy.Application.Members.DTOs;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.LINQ;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Dummy.Application.Members.Queries
{

    public class GetPagingMemberListQuery() : PagingRequestDTO, IRequestWithBaseResponse<PagingResponseDTO<MemberDTO>>;
    internal class GetPagingMemberListQuerytHandler : IRequestWithBaseResponseHandler<GetPagingMemberListQuery, PagingResponseDTO<MemberDTO>>
    {
        private readonly MainDBContext _mainDBContext;

        public GetPagingMemberListQuerytHandler(MainDBContext mainDBContext)
        {
            _mainDBContext = mainDBContext;
        }
        public async Task<BaseResponseDTO<PagingResponseDTO<MemberDTO>>> Handle(GetPagingMemberListQuery request, CancellationToken cancellationToken)
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
                                              })
                                              .OrderByDescending(x => x.CreatedDate);

            // Calculate item length
            var totalMembers = await query.CountAsync(cancellationToken);

            // Perform pagination on item length with request page index and page limit
            var pagedMembers = await query.Page(request.PageIndex, request.PageLimit)
                                          .ToListAsync(cancellationToken);

            // Calculate total pages
            var totalPages = Math.Ceiling((double)totalMembers / request.PageLimit);

            return new BaseResponseDTO<PagingResponseDTO<MemberDTO>>
            {
                Code = HttpStatusCode.OK,
                Message = "Successfully get all memebers",
                Data = new PagingResponseDTO<MemberDTO>
                {
                    PageIndex = request.PageIndex,
                    PageLimit = request.PageLimit,
                    ItemLength = totalMembers,
                    TotalPages = (int)totalPages,
                    Data = pagedMembers
                }
            };
        }
    }
}
