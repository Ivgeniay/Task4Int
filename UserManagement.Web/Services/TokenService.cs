using System.IdentityModel.Tokens.Jwt;
using UserManagement.Core.Interfaces;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Core.Models;
using System.Security.Claims;
using UserManagement.Core;
using System.Text;

namespace UserManagement.Web.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration[WebConstants.Configuration.SECRET]);
            var issuer = _configuration[WebConstants.Configuration.ISSUER];
            var audience = _configuration[WebConstants.Configuration.AUDIENCE];
            var expiryInDays = int.Parse(_configuration[WebConstants.Configuration.EXPIRE_IN_DAYS]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(Constants.Auth.USER_ID_CLAIM, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                Expires = DateTime.UtcNow.AddDays(expiryInDays),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateJwtToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration[WebConstants.Configuration.SECRET]);
            var issuer = _configuration[WebConstants.Configuration.ISSUER];
            var audience = _configuration[WebConstants.Configuration.AUDIENCE];

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken)
                {
                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var userId = int.Parse(jwtToken.Claims.First(x => x.Type == Constants.Auth.USER_ID_CLAIM).Value);

                    return userId;
                }

                throw new SecurityTokenException("Invalid token");
            }
            catch
            {
                return null;
            }
        }
    }
}
