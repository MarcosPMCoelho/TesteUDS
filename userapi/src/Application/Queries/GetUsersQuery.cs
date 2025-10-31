using MediatR;
using UserApi.Application.DTOs;
using System.Collections.Generic;

namespace UserApi.Application.Queries
{
    public record GetUsersQuery() : IRequest<IEnumerable<UserDto>>;
}
