using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private ResponseDto responseDto = new();
        private readonly IAuthService authService;

        public AuthAPIController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var errorMessage = await authService.Register(registrationRequestDto);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                responseDto.IsSuccess = false;
                responseDto.Message = errorMessage;

                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await authService.Login(loginRequestDto);

            if (loginResponse is null || loginResponse.User is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = "Username or password is incorrect";
               
                return BadRequest(responseDto);
            }

            responseDto.Result = loginResponse;

            return Ok(responseDto);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto RegistrationRequestDto)
        {
            if (RegistrationRequestDto.Role is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = "Invalid or null Role";

                return BadRequest(responseDto);
            }

            var assignRoleResponse = await authService.AssignRole(RegistrationRequestDto.Email, RegistrationRequestDto.Role.ToUpper());

            if (!assignRoleResponse)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = "Error encountered assigning role";

                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }
    }
}
