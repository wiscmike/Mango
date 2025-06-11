using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ITokenProvider tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            this.authService = authService;
            this.tokenProvider = tokenProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginRequestDto loginRequestDto = new();

            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var responseDto = await authService.LoginAsync(loginRequestDto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                var loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                await SignInUser(loginResponseDto);

                tokenProvider.SetToken(loginResponseDto.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                //ModelState.AddModelError("CustomError", responseDto.Message);
                TempData["error"] = responseDto is not null ? responseDto.Message : "Unknown login error";
                return View(loginRequestDto);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem(StaticDetail.RoleAdmin, StaticDetail.RoleAdmin),
                new SelectListItem(StaticDetail.RoleCustomer, StaticDetail.RoleCustomer)
            };

            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registerRequestDto)
        {
            var responseDto = await authService.RegisterAsync(registerRequestDto);

            if (responseDto is not null && responseDto.IsSuccess)
            {
                if (!string.IsNullOrEmpty(registerRequestDto.Role))
                {
                    registerRequestDto.Role = StaticDetail.RoleCustomer;
                }

                var roleResult = await authService.AssignRoleAsync(registerRequestDto);

                if (roleResult != null && roleResult.IsSuccess)
                {
                    TempData["success"] = "Registration is successfull!";
                    return RedirectToAction("login");
                }
            }
            else
            {
                TempData["error"] = responseDto is not null ? responseDto.Message : "Unknown registration error";
            }

                var roleList = new List<SelectListItem>()
            {
                new SelectListItem(StaticDetail.RoleAdmin, StaticDetail.RoleAdmin),
                new SelectListItem(StaticDetail.RoleCustomer, StaticDetail.RoleCustomer)
            };

            ViewBag.RoleList = roleList;
            
            return View(registerRequestDto);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResponseDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, 
                    jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, 
                    jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                    jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
                    jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role,
                   jwt.Claims.FirstOrDefault(c => c.Type == "role").Value));
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
