using Xunit;
using Microsoft.EntityFrameworkCore;
using UserApi.Infrastructure;
using UserApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using UserApi.Application.Commands;
using UserApi.Application.Handlers;
using System.Threading;
using System.Threading.Tasks;

public class UserHandlerTests
{
    [Fact]
    public async Task CreateUser_Should_Create_When_Email_Not_Exists()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "testdb1")
            .Options;

        await using var db = new AppDbContext(options);
        var passwordHasher = new PasswordHasher<User>();
        var handler = new CreateUserCommandHandler(db, passwordHasher);

        var dto = await handler.Handle(new CreateUserCommand("Name","a@b.com","123456"), CancellationToken.None);

        Assert.Equal("a@b.com", dto.Email);
    }
}
