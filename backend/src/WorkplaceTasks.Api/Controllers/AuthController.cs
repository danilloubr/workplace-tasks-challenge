using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Application.Interfaces.Services;

namespace WorkplaceTasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {

            var token = await _authService.LoginAsync(loginRequest);
            return Ok(new LoginResponseDto { Token = token });
        }
    }
}