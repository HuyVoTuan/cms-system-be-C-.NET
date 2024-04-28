using Dummy.Domain.Entities;

namespace Dummy.Infrastructure.Services.Auth
{
    public interface IAuthService
    {
        public string HashPassword(string plainPassword);
        public bool VerifyPassword(string plainPassword, string hashedPassword);
        public string GenerateToken(Member member);
        public RefreshToken GenerateRefreshToken(Member member);
    }
}
