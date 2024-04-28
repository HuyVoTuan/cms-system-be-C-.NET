using Dummy.Domain.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dummy.Infrastructure.Services.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid? Id
        {
            get
            {
                var memberId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bool isGuidParsed = Guid.TryParse(memberId, out Guid id);

                if (memberId is null || !isGuidParsed)
                {
                    return null;
                }
                return id;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.IsInRole(MemberRole.Admin) ?? false;
            }
        }
    }
}
