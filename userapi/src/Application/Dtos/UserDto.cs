using System;

namespace UserApi.Application.DTOs
{
    public record UserDto(Guid Id, string Name, string Email, DateTime CreatedAt);
}
