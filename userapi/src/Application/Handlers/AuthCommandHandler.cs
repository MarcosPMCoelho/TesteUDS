using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using UserApi.Application.Commands;
using UserApi.Infrastructure;

namespace UserApi.Application.Handlers
{
    public class AuthCommandHandler
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<UserApi.Domain.Entities.User> _hasher;

        public AuthCommandHandler(AppDbContext db, IConfiguration configuration, Microsoft.AspNetCore.Identity.IPasswordHasher<UserApi.Domain.Entities.User> hasher)
        {
            _db = db;
            _configuration = configuration;
            _hasher = hasher;
        }

        public async Task<string> Handle(AuthCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (res == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed) throw new UnauthorizedAccessException("Invalid credentials");

            var jwt = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwt["Key"]!);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
