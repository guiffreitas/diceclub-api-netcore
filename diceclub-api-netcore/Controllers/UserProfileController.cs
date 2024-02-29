using AutoMapper;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Services;
using diceclub_api_netcore.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace diceclub_api_netcore.Controllers
{
    [ApiController]
    [Route("profile")]
    public class UserProfileController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly UserService userService;
        private readonly UserProfileService profileService;

        public UserProfileController(IMapper mapper,UserService userService, UserProfileService profileService)
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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] UserProfileDto profileDto, string userName) 
        {
            try
            {
                var userId = userService.GetUserIdByUsername(userName);

                if (userId is null)
                {
                    return BadRequest("Invalid username");
                }

                var profile = mapper.Map<UserProfile>(profileDto);

                profile.Id = userId.Id;

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
    }
}
