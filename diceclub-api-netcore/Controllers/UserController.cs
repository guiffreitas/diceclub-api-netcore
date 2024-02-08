using AutoMapper;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace diceclub_backend_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IMapper mapper;
        private readonly IUserService userService;

        public UserController(IMapper mapper, IUserService userService)
        {
            this.mapper = mapper;
            this.userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (!userDto.IsValid)
            {
                return BadRequest("Invalid user information");
            }
            try
            {
                var user = mapper.Map<User>(userDto);

                var result = await userService.RegisterUser(user, userDto.Password);

                if(result.Succeeded)
                {
                    return Ok("User registered");
                }

                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string confirmationTokem)
        {
            if(confirmationTokem != null)
            {
                var result = await userService.ConfirmEmail(userId, confirmationTokem);

                if (result.Succeeded)
                {
                    return Ok("User confirmed");
                }

                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            return BadRequest("Invalid confirmation token");
        }
    }
}
