
using API.Dtos;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController: BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            return new UserDto { Email = user.Email, Username = user.DisplayName, Token = _tokenService.CreateToken(user) };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> register(RegisterDto registerDto)
        {
            var user = new AppUser { DisplayName = registerDto.DisplayName, Email = registerDto.Email, UserName = registerDto.Email };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            return new UserDto { Email = user.Email, Username = user.UserName, Token = _tokenService.CreateToken(user) };
        }
    }
}
