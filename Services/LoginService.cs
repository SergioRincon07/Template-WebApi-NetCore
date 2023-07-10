using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.DataBaseMegaLinea.Context;
using WebApi.DataBaseMegaLinea.Models;
using WebApi.Models;

namespace WebApi.Services
{
    public class LoginService
    {
        private readonly MegaLineaContext _context;

        public LoginService(MegaLineaContext context)
        {
            _context = context;

        }
        public async Task<List<spLoginResult>> LoginAppMegaLinea(UsuarioLogin usuarioLogin)
        {
            List<spLoginResult> ListUserLogin = await _context.GetProcedures().spLoginAsync(usuarioLogin.Username, usuarioLogin.Password);
            return ListUserLogin;
        }
        public static string GenerateToken(IConfiguration configuration)
        {
            var jwt = configuration.GetSection("Jwt").Get<Jwt>();

            var now = DateTime.UtcNow;
            var expiresToken = DateTime.UtcNow.AddHours(24);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("startToken", now.ToString("yyyy-MM-dd HH:mm:ss")),
                new Claim("expiresToken", expiresToken.ToString("yyyy-MM-dd HH:mm:ss")),
                new Claim("username", "username")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(jwt.Issuer, jwt.Audience, claims, expires: expiresToken, signingCredentials: signing);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }


    }
}
