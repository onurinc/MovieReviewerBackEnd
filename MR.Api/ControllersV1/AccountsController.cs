using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MR.DataAccessLayer.Interfaces;
using MR.LogicLayer.Configuration;
using MR.LogicLayer.Models.DTOs.Incoming;
using MR.LogicLayer.Models.DTOs.Outgoing;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionMonitor) : base(unitOfWork)
        {
            _userManager = userManager;
            _jwtConfig = optionMonitor.CurrentValue;
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
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    }),
                Expires = DateTime.UtcNow.AddHours(3),
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