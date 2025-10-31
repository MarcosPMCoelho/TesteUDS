using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApi.Application.Commands;
using UserApi.Application.DTOs;
using UserApi.Application.Queries;
using UserApi.Domain.Entities;
using UserApi.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace UserApi.Application.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public CreateUserCommandHandler(AppDbContext db, IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            if (await _db.Users.AnyAsync(u => u.Email == email, cancellationToken))
                throw new InvalidOperationException("Email already registered");

            var user = new User(request.Name.Trim(), email, string.Empty);
            var hash = _passwordHasher.HashPassword(user, request.Password);
            user.UpdatePasswordHash(hash);

            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.Name, user.Email, user.CreatedAt);
        }
    }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
    {
        private readonly AppDbContext _db;
        public GetUsersQueryHandler(AppDbContext db) => _db = db;

        public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return await _db.Users
                .OrderBy(u => u.CreatedAt)
                .Select(u => new UserDto(u.Id, u.Name, u.Email, u.CreatedAt))
                .ToListAsync(cancellationToken);
        }
    }
}
