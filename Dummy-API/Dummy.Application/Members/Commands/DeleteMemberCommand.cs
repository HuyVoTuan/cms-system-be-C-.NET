using Dummy.Infrastructure;
using Dummy.Infrastructure.Commons;
using Dummy.Infrastructure.Commons.Base;
using Dummy.Infrastructure.Extensions;
using Dummy.Infrastructure.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Net;

namespace Dummy.Application.Members.Commands
{
    public record DeleteMemberCommand(String Slug) : IRequestWithBaseResponse;

    internal class DeleteMemberCommandHandler : IRequestWithBaseResponseHandler<DeleteMemberCommand>
    {
        private readonly MainDBContext _mainDBContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<DeleteMemberCommandHandler> _localizer;

        public DeleteMemberCommandHandler(MainDBContext mainDBContext,
        IStringLocalizer<DeleteMemberCommandHandler> localizer, ICurrentUserService currentUserService)
        {
            _localizer = localizer;
            _mainDBContext = mainDBContext;
            _currentUserService = currentUserService;
        }
        public async Task<BaseResponseDTO> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
        {
            var isAdmin = _currentUserService.IsAdmin;

            if (!isAdmin)
            {
                throw new RestfulAPIException(HttpStatusCode.Forbidden, _localizer.Translate("failure.cant_perform", new List<String> { "user" }));
            }

            var existingMember = await _mainDBContext.Members.FirstOrDefaultAsync(x => x.Slug == request.Slug, cancellationToken);

            if (existingMember is null)
            {
                throw new RestfulAPIException(HttpStatusCode.NotFound, _localizer.Translate("failure.not_exists", new List<String> { "user" }));
            }

            _mainDBContext.Members.Remove(existingMember);
            await _mainDBContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO(HttpStatusCode.NoContent);
        }
    }
}
