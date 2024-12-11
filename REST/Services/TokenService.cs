using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using REST.Interfaces;
using System.IdentityModel.Tokens.Jwt;
namespace REST.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;            
        }
        public string GenerateJWT(string username)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var expiration = DateTime.UtcNow.AddMinutes(10);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: expiration,
                claims: new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                },
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            var accessToken = tokenHandler.WriteToken(token);
            return accessToken;
        }
    }
}