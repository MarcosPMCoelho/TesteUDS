using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using UserApi.Infrastructure;
using UserApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using UserApi.Application.Handlers;
using UserApi.Application.Commands;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class AuthHandlerTest
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IConfiguration GetFakeConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Key", "TESTE_SECRET_JWT_KEY_12345678901234567890"},
            {"Jwt:Issuer", "UserApi"},
            {"Jwt:Audience", "UserApiUsers"}
        };
        return new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();
    }

    [Fact]
    public async System.Threading.Tasks.Task Should_Return_Valid_Jwt_Token_When_User_Is_Authenticated()
    {
        var db = GetInMemoryDbContext();
        var config = GetFakeConfiguration();
        var passwordHasher = new PasswordHasher<User>();

        var rawPassword = "123456";
        var user = new User("User Test", "user@test.com", string.Empty);
        var hash = passwordHasher.HashPassword(user, rawPassword);
        user.UpdatePasswordHash(hash);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new AuthCommandHandler(db, config, passwordHasher);
        var token = await handler.Handle(new AuthCommand { Email = "user@test.com", Password = rawPassword }, default);

        Assert.False(string.IsNullOrEmpty(token));

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "UserApi",
            ValidAudience = "UserApiUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TESTE_SECRET_JWT_KEY_12345678901234567890"))
        };

        tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        Assert.NotNull(validatedToken);
    }
}
