using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningPlatform.Services.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }


            var response = await authService.RegisterAsync(registerDto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }


            var user = await authService.AuthenticateAsync(loginDto);

            return Ok(user);
        }
    }
}
