using AutoMapper;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Domain.Services;
using diceclub_api_netcore.Dtos;
using diceclub_api_netcore.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace diceclub_api_netcore.Controllers
{
    [ApiController]
    [Route("profile")]
    public class UserProfileController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IUserProfileService profileService;

        public UserProfileController(IMapper mapper, IUserService userService, IUserProfileService profileService)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.profileService = profileService;
        }

        /// <summary>
        /// Create user profile
        /// </summary>
        /// <param name="profileDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] UserProfileDto profileDto, [Required] string userName) 
        {
            try
            {
                var userId = await userService.GetUserIdByUsername(userName);

                if (userId is null)
                {
                    return Problem(title: nameof(userName), detail: "username not found", statusCode: 404);
                }

                var profile = mapper.Map<UserProfile>(profileDto);

                profile.UpdateTime = DateTime.UtcNow;

                profile.UserId = (int)userId;

                var cancelattionToken = new CancellationToken();

                var result = await profileService.CreateUserProfile(profile, cancelattionToken);

                if (result.Success)
                {
                    return Ok();
                }

                return Problem(result.Message, statusCode: 500);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: 500);
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <param name="profileDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] UserProfileDto profileDto, [Required] string userName)
        {
            try
            {
                var userId = await userService.GetUserIdByUsername(userName);

                if (userId is null)
                {
                    return Problem(title: nameof(userName), detail: "username not found", statusCode: 404);
                }

                var profile = mapper.Map<UserProfile>(profileDto);

                profile.UpdateTime = DateTime.UtcNow;

                profile.UserId = (int)userId;

                var cancelattionToken = new CancellationToken();

                var result = await profileService.UpdateUserProfile(profile, cancelattionToken);

                if (result.Success)
                {
                    return Ok();
                }

                return Problem(result.Message, statusCode: 500);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: 500);
            }
        }

        /// <summary>
        /// Get user profile
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([Required] string userName)
        {
            try
            {
                var userId = await userService.GetUserIdByUsername(userName);

                if (userId is null)
                {
                    return Problem(title: nameof(userName), detail: "username not found", statusCode: 404);
                }

                var cancelattionToken = new CancellationToken();

                var result = await profileService.GetUserProfile((int)userId, cancelattionToken);

                if (result.Success)
                {
                    var profileDto = mapper.Map<UserProfileDto>(result.Result);

                    return Ok(profileDto);
                }

                return Problem(title: nameof(UserProfile), detail: "user profile not found", statusCode: 404);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: 500);
            }
        }
    }
}
