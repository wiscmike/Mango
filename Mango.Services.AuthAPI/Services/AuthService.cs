using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Configuration;

namespace Mango.Services.AuthAPI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext appDbContext;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IJwtTokenGenerator jwtTokenGenerator;
    private readonly ILogger<AuthService> logger;

    public AuthService(AppDbContext appDbContext,
                       UserManager<ApplicationUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       IJwtTokenGenerator jwtTokenGenerator,
                       ILogger<AuthService> logger)
    {
        this.appDbContext = appDbContext;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.jwtTokenGenerator = jwtTokenGenerator;
        this.logger = logger;
    }
    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        try
        {
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName!.ToLower().Equals(loginRequestDto.UserName.ToLower()));

            if (user is not null)
            {
                var isValid = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                if (!isValid)
                {
                    // invalid password
                    logger.LogError("Invalid password");
                    return SetNullUserForReturn();
                }

                // get the role(s) associated with the user
                var roles = await userManager.GetRolesAsync(user);
                var token = jwtTokenGenerator.GenerateToken(user, roles);

                if (token != null)
                {
                    UserDto userDto = new()
                    {
                        Email = user.Email + string.Empty,
                        Name = user.Name,
                        Id = user.Id,
                        PhoneNumber = user.PhoneNumber + string.Empty
                    };

                    LoginResponseDto loginResponseDto = new()
                    {
                        User = userDto,
                        Token = token
                    };

                    logger.LogInformation("Successfully logged in user.");

                    return loginResponseDto;
                }
                // null token
                logger.LogError("Invalid token");
                return SetNullUserForReturn();
            }
            else
            {
                // null application user
                logger.LogError("Null value for application user");
                return SetNullUserForReturn();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return SetNullUserForReturn();
        }

        static LoginResponseDto SetNullUserForReturn()
        {
            return new LoginResponseDto() { User = null, Token = string.Empty };
        }

    }

    public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
    {
        ApplicationUser user = new ()
        {
            UserName = registrationRequestDto.Email,
            Email = registrationRequestDto.Email,
            NormalizedEmail = registrationRequestDto.Email.ToUpper(),
            Name = registrationRequestDto.Name,
            PhoneNumber = registrationRequestDto.PhoneNumber
        };

        try
        {
            var result = await userManager.CreateAsync(user, registrationRequestDto.Password);
            if (result.Succeeded)
            {
                logger.LogInformation("User successfully registered");

                // not required since returning a string
                var userToReturn = await appDbContext.ApplicationUsers.FirstAsync(u => u.UserName == registrationRequestDto.Email);

                // not required since returning a string
                UserDto userDto = new()
                {
                    Email = userToReturn.Email + string.Empty,
                    Name = userToReturn.Name,
                    Id = userToReturn.Id,
                    PhoneNumber = userToReturn.PhoneNumber + string.Empty
                };

                logger.LogInformation("Successfully registered new user.");

                return string.Empty;
            }
            else
            {
                if (result.Errors.Any())
                {
                    return result.Errors.First().Description;
                }
                return "Unknown registration error";
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return ex.Message;
        }
    }

    public async Task<bool> AssignRole(string email, string roleName)
    {
        try
        {
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email!.ToLower().Equals(email.ToLower()));

            if (user != null)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // create the new role
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }

                await userManager.AddToRoleAsync(user, roleName);

                logger.LogInformation($"Successfully assigned role {roleName}");
                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError (ex.Message);
        }

        return false;
    }


}
