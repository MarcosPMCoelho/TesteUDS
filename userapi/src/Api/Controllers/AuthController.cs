using Microsoft.AspNetCore.Mvc;
using UserApi.Application.Commands;
using UserApi.Application.Handlers;

namespace UserApi.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthCommandHandler _authHandler;
        public AuthController(AuthCommandHandler authHandler) => _authHandler = authHandler;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthCommand cmd)
        {
            try
            {
                var token = await _authHandler.Handle(cmd, default);
                return Ok(new { token });
            }
            catch
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }
    }
}
