using MediatR;
using UserApi.Application.DTOs;

namespace UserApi.Application.Commands
{
    public record CreateUserCommand(string Name, string Email, string Password) : IRequest<UserDto>;
}
