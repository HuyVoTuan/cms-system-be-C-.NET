using Dummy.Application.Members.Commands;
using Dummy.Application.Members.DTOs;
using Dummy.Application.Members.Queries;
using Dummy.Infrastructure.Commons.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dummy.API.Controllers
{
    [Route("api/members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MembersController(IMediator mediator)
        {
            _mediator = mediator;
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
        [HttpGet("/api/member/")]
        public async Task<IActionResult> GetCurrentMember([FromQuery] GetCurrentMemberQuery request, CancellationToken cancellationToken)
        {
            var getCurrentMemberResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO<MemberDTO>>
            {
                StatusCode = getCurrentMemberResponse.Code,
                Data = getCurrentMemberResponse,
            };
        }

        [HttpPost("/api/member/forgot-password")]
        public async Task<IActionResult> ResetCurrentMemberPassword([FromBody] ResetCurrentMemberPasswordCommand request, CancellationToken cancellationToken)
        {
            var resetCurrentMemberPasswordResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO>
            {
                StatusCode = resetCurrentMemberPasswordResponse.Code,
                Data = resetCurrentMemberPasswordResponse,
            };
        }

        [Authorize]
        [HttpPut("/api/member")]
        public async Task<IActionResult> UpsertCurrentMemberDetailAndLocation([FromBody] UpsertCurrentMemberDetailAndLocationCommand request, CancellationToken cancellationToken)
        {
            var upsertCurrentMemberDetailAndLocationResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO<UpsertCurrentMemberDetailAndLocationDTO>>
            {
                StatusCode = upsertCurrentMemberDetailAndLocationResponse.Code,
                Data = upsertCurrentMemberDetailAndLocationResponse
            };
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{slug}")]
        public async Task<IActionResult> DeleteMember([FromRoute] String slug, CancellationToken cancellationToken)
        {
            var request = new DeleteMemberCommand(slug);
            var deleteMemberResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO>
            {
                StatusCode = deleteMemberResponse.Code
            };
        }

        [Authorize]
        [HttpPost("/api/member/refresh-token")]
        public async Task<IActionResult> RefreshCurrentMemberToken([FromBody] RefreshTokenMemberCommand request, CancellationToken cancellationToken)
        {
            var refreshCurrentMemberTokenResult = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO<AuthResponseDTO>>
            {
                StatusCode = refreshCurrentMemberTokenResult.Code,
                Data = refreshCurrentMemberTokenResult,
            };
        }

        [Authorize]
        [HttpDelete("/api/member/revoke-token")]
        public async Task<IActionResult> RevokeCurrentMemberRefreshToken([FromBody] RevokeCurrentMemberRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var revokeRefreshTokenResponse = await _mediator.Send(request, cancellationToken);

            return new CustomActionResult<BaseResponseDTO>
            {
                StatusCode = revokeRefreshTokenResponse.Code
            };
        }
    }
}
