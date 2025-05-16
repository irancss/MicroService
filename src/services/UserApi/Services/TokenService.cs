using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace User.Services
{
    
    public interface ITokenService
    {
        string GenerateToken(Models.User user, IList<string> roles);
    }


    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; }
    }

    public class TokenService : ITokenService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly JwtSettings _jwt;

        public TokenService(IConnectionMultiplexer redis, JwtSettings jwt)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis)); ;
            _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt)); 
        }

        public string GenerateToken(Models.User user, IList<string> roles)
        {

            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(user.Id)) throw new ArgumentException("User Id is required");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
               // new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles ?? Enumerable.Empty<string>())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expire = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expire,
                signingCredentials: creds
            );


            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var db = _redis.GetDatabase();
            db.StringSet($"token:{user.Id}", jwtToken, TimeSpan.FromHours(1));
            db.StringSet($"token:{user.Id}", jwtToken, TimeSpan.FromHours(1));
            return jwtToken; // فقط یک return مجاز است
        }

    }
}
