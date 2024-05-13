using Dummy.Application.Members.Commands;
using Dummy.Application.Members.DTOs;
using Dummy.Application.Members.Queries;
using Dummy.Infrastructure.Commons.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Dummy.API.Controllers
{
    [Route("api/member")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer _localizer;

        public MembersController(IMediator mediator, IStringLocalizer localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> GetMemberList([FromQuery] GetPagingMemberListQuery request, CancellationToken cancellationToken)
        {
            var getMemberListResponse = await _mediator.Send(request, cancellationToken);
            return new CustomActionResult<BaseResponseDTO<PagingResponseDTO<MemberDTO>>>
            {
                StatusCode = getMemberListResponse.Code,
                Data = getMemberListResponse,
            };
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetSingleMember([FromRoute] String slug, CancellationToken cancellationToken)
        {
            var query = new GetSingleMemberQuery(slug);
            var getSingleMemberResponse = await _mediator.Send(query, cancellationToken);

            return new CustomActionResult<BaseResponseDTO<MemberDTO>>
            {
                StatusCode = getSingleMemberResponse.Code,
                Data = getSingleMemberResponse,
            };
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterMember([FromBody] RegisterMemberCommand request, CancellationToken cancellationToken)
        {
            var registerMemberResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO<AuthResponseDTO>>
            {
                StatusCode = registerMemberResponse.Code,
                Data = registerMemberResponse,
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginMember([FromBody] LoginMemberCommand request, CancellationToken cancellationToken)
        {
            var loginMemberResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO<AuthResponseDTO>>
            {
                StatusCode = loginMemberResponse.Code,
                Data = loginMemberResponse,
            };
        }

        [Authorize]
        [HttpPut("{slug}")]
        public async Task<IActionResult> UpsertMemberDetailAndLocation([FromRoute] String slug,
                                                                       [FromBody] UpsertMemberDetailAndLocationCommandRequestDTO requestDTO,
                                                                       CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest($"{_localizer["slug"]} {_localizer["failure.cant_be_empty"]}");
            }

            var request = new UpsertMemberDetailAndLocationCommand
            {
                Slug = slug,
                FirstName = requestDTO.FirstName,
                LastName = requestDTO.LastName,
                Position = requestDTO.Position,
                Avatar = requestDTO.Avatar,
                Address = requestDTO.Address,
                City = requestDTO.City,
                District = requestDTO.District
            };

            var upsertMemberDetailAndLocationResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO<UpsertMemberDetailAndLocationDTO>>
            {
                StatusCode = upsertMemberDetailAndLocationResponse.Code,
                Data = upsertMemberDetailAndLocationResponse
            };
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{slug}")]
        public async Task<IActionResult> DeleteMember([FromRoute] String slug, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest($"{_localizer["slug"]} {_localizer["failure.cant_be_empty"]}");
            }

            var request = new DeleteMemberCommand(slug);
            var deleteMemberResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO>
            {
                StatusCode = deleteMemberResponse.Code
            };
        }

        [Authorize]
        [HttpPost("{slug}/refresh-token")]
        public async Task<IActionResult> RefreshTokenMember([FromRoute] String slug, [FromBody] RefreshTokenMemberCommandRequestDTO requestDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest($"{_localizer["slug"]} {_localizer["failure.cant_be_empty"]}");
            }

            var request = new RefreshTokenMemberCommand
            {
                Slug = slug,
                RefreshToken = requestDTO.RefreshToken,
            };

            var refreshTokenMemberResult = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO<AuthResponseDTO>>
            {
                StatusCode = refreshTokenMemberResult.Code,
                Data = refreshTokenMemberResult,
            };
        }

        [Authorize]
        [HttpDelete("{slug}/revoke-token")]
        public async Task<IActionResult> RevokeMemberRefreshToken([FromRoute] String slug, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest($"{_localizer["slug"]} {_localizer["failure.cant_be_empty"]}");
            }

            var request = new RevokeMemberRefreshTokenCommand(slug);
            var revokeRefreshTokenResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO>
            {
                StatusCode = revokeRefreshTokenResponse.Code
            };
        }

    }
}
