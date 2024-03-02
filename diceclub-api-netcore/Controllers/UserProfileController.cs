using AutoMapper;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Domain.Services;
using diceclub_api_netcore.Dtos;
using diceclub_api_netcore.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace diceclub_api_netcore.Controllers
{
    [ApiController]
    [Route("profile")]
    public class UserProfileController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IUserProfileService profileService;
        private readonly ApplicationDbContext context;

        public UserProfileController(IMapper mapper, IUserService userService, IUserProfileService profileService, ApplicationDbContext context)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.profileService = profileService;
            this.context = context;
        }

        /// <summary>
        /// Create user profile
        /// </summary>
        /// <param name="profileDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] UserProfileDto profileDto, string userName) 
        {
            try
            {
                var userId = await userService.GetUserIdByUsername(userName);

                if (userId is null)
                {
                    return BadRequest("Invalid username");
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

                return BadRequest(result.Message.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UserProfileDto profileDto, string userName)
        {
            try
            {
                var userId = await userService.GetUserIdByUsername(userName);

                if (userId is null)
                {
                    return BadRequest("Invalid username");
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

                return BadRequest(result.Message.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
