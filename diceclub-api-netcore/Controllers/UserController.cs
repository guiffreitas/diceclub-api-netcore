using AutoMapper;
using diceclub_api_netcore.Domain.Entities;
using diceclub_api_netcore.Domain.Interfaces.Services;
using diceclub_api_netcore.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Route for user register
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([Required] UserDto userDto)
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

        /// <summary>
        /// Confirm user email after registered
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confirmationTokem"></param>
        /// <returns></returns>
        [HttpPost("confirmEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([Required] string userId, [Required] string confirmationTokem)
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

        /// <summary>
        /// Login for users
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([Required] string email, [Required] string password)
        {
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                var (result, userToken) = await userService.LoginUser(email, password);

                if (result.Succeeded)
                {
                    return Ok(userToken);
                }

                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }

            return BadRequest("Invalid user information");
        }
    }
}
