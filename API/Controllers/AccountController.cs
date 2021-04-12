
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailFromClaimPrincile(User);
            return new UserDto 
            { 
                Email = user.Email, 
                Username = user.UserName, 
                Token = _tokenService.CreateToken(user) 
            };
        }

        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExistAsync([FromQuery] string email)
        {
            return (await _userManager.FindByEmailAsync(email)) != null;
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAdress()
        {
            var user = await _userManager.FindByClaimPrincipleWithAddressAsync(User);

            return _mapper.Map<AddressDto>(user.Address);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto dto)
        {
            var user = await _userManager.FindByClaimPrincipleWithAddressAsync(User);

            user.Address = _mapper.Map<Address>(dto);

            var result = await _userManager.UpdateAsync(user);

            return Ok(_mapper.Map<Address>(user.Address));
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
            if(CheckEmailExistAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse { 
                    Errors = new[] { "Email is in use" } 
                });
            }

            var user = new AppUser { DisplayName = registerDto.DisplayName, Email = registerDto.Email, UserName = registerDto.Email };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            return new UserDto { Email = user.Email, Username = user.UserName, Token = _tokenService.CreateToken(user) };
        }
    }
}
