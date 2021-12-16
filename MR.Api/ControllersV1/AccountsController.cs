using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Interfaces;
using MR.LogicLayer.Configuration;
using MR.DataAccessLayer.Entities.DTOs.Incoming;
using MR.DataAccessLayer.Entities.DTOs.Outgoing;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace MR.Api.ControllersV1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseController
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly JwtConfig _jwtConfig;

        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            TokenValidationParameters tokenValidationParameters,
            IOptionsMonitor<JwtConfig> optionMonitor) : base(unitOfWork, userManager)
        {
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
                    EmailConfirmed = true, // ToDo build email confirmation
                };

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

                return Ok(new UserRegistrationResponseDto()
                {
                    Succes = true,
                    Token = token
                });
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
                    return Ok(new UserLoginResponseDto()
                    {
                        Succes = true,
                        Token = jwtToken
                    });
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

        private string GenerateJwtToken(IdentityUser user)
        {
            // the handler is going to be responsible for creating the token
            var jwtHandler = new JwtSecurityTokenHandler();


            // get security key
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("Id", user.Id),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    }),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                    )
            };
            // generate obj token
            var token = jwtHandler.CreateToken(tokenDescriptor);
            // convert to string
            var jwtToken = jwtHandler.WriteToken(token);

            return jwtToken;
        }
    }
}