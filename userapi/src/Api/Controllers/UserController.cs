using Microsoft.AspNetCore.Mvc;
using MediatR;
using UserApi.Application.Commands;
using UserApi.Application.Queries;
using UserApi.Application.DTOs;

namespace UserApi.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserCommand cmd)
        {
            try
            {
                var user = await _mediator.Send(cmd);
                return CreatedAtAction(null, new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get()
        {
            var users = await _mediator.Send(new GetUsersQuery());
            return Ok(users);
        }
    }
}
