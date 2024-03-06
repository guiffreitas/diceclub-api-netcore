using AutoMapper;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;

namespace diceclub_api_netcore.Controllers
{
    [ApiController]
    [Route("profile")]
    public class UserProfileController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly ITokenService tokenService;
        private readonly IUserProfileService profileService;

        public UserProfileController(IMapper mapper, IUserService userService, ITokenService tokenService, IUserProfileService profileService)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.tokenService = tokenService;
            this.profileService = profileService;
        }

        /// <summary>
        /// Create user profile
        /// </summary>
        /// <param name="profileDto"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] UserProfileDto profileDto) 
        {
            try
            {
                var userId = await GetUserIdFromToken();

                if (userId is null)
                {
                    return Problem(title: "user", detail: "user not found", statusCode: 404);
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
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UserProfileDto profileDto)
        {
            try
            {
                var userId = await GetUserIdFromToken();

                if (userId is null)
                {
                    return Problem(title: "user", detail: "user not found", statusCode: 404);
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
        /// <returns></returns>
        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = await GetUserIdFromToken();

                if (userId is null)
                {
                    return Problem(title: "user", detail: "user not found", statusCode: 404);
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

        private async Task<int?> GetUserIdFromToken()
        {
            var userToken = Request.Headers[HeaderNames.Authorization].FirstOrDefault()?.Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(userToken))
            {
                return null;
            }

            var userName = tokenService.GetUsernameFromToken(userToken!);

            return await userService.GetUserIdByUsername(userName!); 
        }
    }
}
