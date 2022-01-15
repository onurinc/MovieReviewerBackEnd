using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Interfaces;
using MR.Api.Configuration;
using MR.DataAccessLayer.Entities.DTOs.Incoming;
using MR.DataAccessLayer.Entities.DTOs.Outgoing;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MR.Api.ControllersV1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseController
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly JwtConfig _jwtConfig;
        private readonly ILogger<AccountsController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            TokenValidationParameters tokenValidationParameters,
            IOptionsMonitor<JwtConfig> optionMonitor,
            ILogger<AccountsController> logger,
            RoleManager<IdentityRole> roleManager) : base(unitOfWork, userManager)
        {
            _roleManager = roleManager;
            _logger = logger;
            _jwtConfig = optionMonitor.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationDto)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(registrationDto.Email);

                if (userExist != null)
                {
                    return BadRequest(new UserRegistrationResponseDto()
                    {
                        Succes = false,
                        Errors = new List<string>()
                        {
                            "Email already in use"
                        }
                    });
                }

                var newUser = new IdentityUser()
                {
                    Email = registrationDto.Email,
                    UserName = registrationDto.Email,
                    EmailConfirmed = true,
                };
                await _userManager.AddToRoleAsync(newUser, "AppUser");
                var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);
                if (!isCreated.Succeeded) // When register failed
                {
                    return BadRequest(new UserRegistrationResponseDto()
                    {
                        Succes = isCreated.Succeeded,
                        Errors = isCreated.Errors.Select(x => x.Description).ToList()
                    });
                }

                var _user = new User();
                _user.IdentityId = new Guid(newUser.Id);
                _user.FirstName = registrationDto.FirstName;
                _user.LastName = registrationDto.LastName;
                _user.Email = registrationDto.Email;
                _user.Status = 1;

                await _unitOfWork.Users.Add(_user);
                await _unitOfWork.CompleteAsync();


                var token = GenerateJwtToken(newUser);

                return Ok(token);
            }

            else
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Succes = false,
                    Errors = new List<string>(){
                    "Invalid payload"
                }
                });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginDto)
        {
            if (ModelState.IsValid)
            {

                // check if email exist 


                var userExist = await _userManager.FindByEmailAsync(loginDto.Email);
                if (userExist == null)
                {
                    return BadRequest(new UserLoginResponseDto()
                    {
                        Succes = false,
                        Errors = new List<string>()
                        {
                            "Invalid authentication request"
                        }
                    });
                }

                // check if user has valid password
                var isCorrect = await _userManager.CheckPasswordAsync(userExist, loginDto.Password);
                if (isCorrect)
                {
                    var jwtToken = GenerateJwtToken(userExist);
                    return Ok(jwtToken);
                }

                else
                {
                    return BadRequest(new UserLoginResponseDto()
                    {
                        Succes = false,
                        Errors = new List<string>()
                        {
                            "Password doesnt match"
                        }
                    });
                }
            }
            else
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Succes = false,
                    Errors = new List<string>(){
                    "Invalid payload"
                }
                });
            }
        }

        private async Task<List<Claim>> GetAllValidClaims(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Getting the claims that we have assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Get the user role and add it to the claims
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;
        }

        private async Task<AuthResult> GenerateJwtToken(IdentityUser user)
        {
            // the handler is going to be responsible for creating the token
            var jwtHandler = new JwtSecurityTokenHandler();


            // get security key
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);


            var claims = await GetAllValidClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                    )
            };
            // generate obj token
            var token = jwtHandler.CreateToken(tokenDescriptor);
            // convert to string
            var jwtToken = jwtHandler.WriteToken(token);

            return new AuthResult()
            {
                Token = jwtToken,
                Succes = true
            };
        }
    }
}