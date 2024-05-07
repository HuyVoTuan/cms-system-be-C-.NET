using Dummy.Application.Locations.DTOs;
using Dummy.Application.Members.DTOs;
using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.LINQ;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Queries
{

    public class GetPagingMemberListQuery() : PagingRequestDTO, IRequestWithBaseResponse<PagingResponseDTO<MemberDTO>>;

    // Command validation
    public class GetPagingMemberListQueryValidator : AbstractValidator<GetPagingMemberListQuery>
    {
        private readonly IStringLocalizer _localizer;


        public GetPagingMemberListQueryValidator(IStringLocalizer localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.PageIndex).Must(x => int.TryParse(x, out var result) && result > 0)
                                     .OverridePropertyName(_localizer["paging.page_index"])
                                     .WithMessage(_localizer["invalid"]);

            RuleFor(x => x.PageLimit).Must(x => int.TryParse(x, out var result) && result > 0)
                                     .OverridePropertyName(_localizer["paging.page_limit"])
                                     .WithMessage(_localizer["invalid"]);
        }
    }

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

            // Convert string paging request fields to integer
            int.TryParse(request.PageIndex, out var pageIndex);
            int.TryParse(request.PageLimit, out var pageLimit);

            // Perform pagination on item length with request page index and page limit
            var pagedMembers = await query.Page(pageIndex, pageLimit)
                                          .ToListAsync(cancellationToken);

            // Calculate total pages
            var totalPages = Math.Ceiling((double)totalMembers / pageLimit);

            return new BaseResponseDTO<PagingResponseDTO<MemberDTO>>
            {
                Code = HttpStatusCode.OK,
                Message = "Successfully get all memebers",
                Data = new PagingResponseDTO<MemberDTO>
                {
                    PageIndex = pageIndex,
                    PageLimit = pageLimit,
                    ItemLength = totalMembers,
                    TotalPages = (int)totalPages,
                    Data = pagedMembers
                }
            };
        }
    }
}
