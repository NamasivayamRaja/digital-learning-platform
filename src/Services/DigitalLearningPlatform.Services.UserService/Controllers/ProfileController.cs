using DigitalLearningPlatform.BuildingBlocks.Common.Extensions;
using DigitalLearningPlatform.Services.UserService.Application.DTOs;
using DigitalLearningPlatform.Services.UserService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningPlatform.Services.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController(IProfileService profileService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            var userId = User.GetUserId();

            var profile = await profileService.GetCurrentUserProfileAsync(userId);

            if (profile == null)
            {
                return NotFound();                
            }

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var userId = User.GetUserId();
            await profileService.UpdateProfileAsync(userId, updateProfileDto);
            return NoContent();
        }
    }
}
