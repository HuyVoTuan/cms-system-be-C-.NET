using Dummy.Domain.Entities;
using Dummy.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dummy.Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private const int ACCESS_TOKEN_LIFE_TIME = 5;
        private const int REFRESH_TOKEN_LIFE_TIME = ACCESS_TOKEN_LIFE_TIME * 12;
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public RefreshToken GenerateRefreshToken(Member member)
        {
            string uniqueResfreshToken = StringHelper.GenerateRefreshToken();

            return new RefreshToken
            {
                MemberId = member.Id,
                Token = uniqueResfreshToken,
                ExpiredDate = DateTime.UtcNow.AddMinutes(REFRESH_TOKEN_LIFE_TIME),
            };
        }

        public string GenerateToken(Member member)
        {
            var credential = _configuration["AppCredential"];
            var key = Encoding.UTF8.GetBytes(credential);

            // Create a token structure base
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, $"{member.Id}"),
                    new Claim(ClaimTypes.Role, $"{member.Position}")
                }),
                Expires = DateTime.UtcNow.AddMinutes(ACCESS_TOKEN_LIFE_TIME),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };

            // Token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create token and write to string
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, 12);
        }

        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}
