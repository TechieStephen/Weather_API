using weatherapi.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace weatherapi.Services.Authentication
{
    public class JwtAuthentication : IJwtAuthentication
    {
        private readonly IConfiguration _config;

        public JwtAuthentication(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(ApplicationUser user, IList<string> userRoles)
        {
            if (user != null)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var claims = new ClaimsIdentity(authClaims);


                ////OR THIS
                //var claims = new ClaimsIdentity(new Claim[]
                //{
                //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //    new Claim(ClaimTypes.Name, user.UserName),
                //    new Claim(ClaimTypes.Email, user.Email),
                //    //new Claim(ClaimTypes.Role, user.Role)

                //});



                var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = claims,
                    SigningCredentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _config["Jwt:Issuer"],
                    Audience = _config["Jwt:Audience"],
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                    //Expires = DateTime.UtcNow.AddHours(7)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                if (token != null)
                {
                    return tokenHandler.WriteToken(token);
                }
            }

            return null;
        }

        public string GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
