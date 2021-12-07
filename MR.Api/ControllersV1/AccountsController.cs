using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Interfaces;
using MR.LogicLayer.Configuration;
using MR.LogicLayer.Models.DTOs.Generic;
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


                var token = await GenerateJwtToken(newUser);

                return Ok(new UserRegistrationResponseDto()
                {
                    Succes = true,
                    Token = token.JwtToken,
                    RefreshToken = token.RefreshToken
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
                    var jwtToken = await GenerateJwtToken(userExist);
                    return Ok(new UserLoginResponseDto()
                    {
                        Succes = true,
                        Token = jwtToken.JwtToken,
                        RefreshToken = jwtToken.RefreshToken
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

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            if (ModelState.IsValid)
            {
                // Check if token is valid
                var result = await VerifyToken(tokenRequestDto);

                if (result == null)
                {
                    return BadRequest(new UserRegistrationResponseDto
                    {
                        Succes = false,
                        Errors = new List<string>()
                        {
                            "Token validation failed"
                        }
                    });
                }
                return Ok(result);

            }
            else // Invalid Object
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

        private async Task<AuthResult> VerifyToken(TokenRequestDto tokenRequestDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Need to check if its an actual token
                var principal = tokenHandler.ValidateToken(tokenRequestDto.Token, _tokenValidationParameters, out var validatedToken);

                // Need to validate the results that have been generated for  us
                // Validate if the string is an actual JWT token and not a random string
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    // Check if token is created with the same algo as our own JWT token
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                        return null;
                }
                    // Check Expiry Date of the token
                    var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                    // Convert to date to cehck
                    var expDate = UnixTimeStampToDateTime(utcExpiryDate);

                    // Checking if JWT Token has expired
                    if (expDate > DateTime.UtcNow)
                    {
                        return new AuthResult()
                        {
                            Succes = false,
                            Errors = new List<string>()
                            {
                                "JWT Token has not expired"
                            }
                        };
                    }

                    // Check if RefreshToken exists in the db
                    var refreshTokenExist = await _unitOfWork.RefreshTokens.GetByRefreshToken(tokenRequestDto.RefreshToken);

                    if (refreshTokenExist == null)
                    {
                        return new AuthResult()
                        {
                            Succes = false,
                            Errors = new List<string>()
                            {
                                "Invalid Refresh Token"
                            }
                        };
                    }

                    // Check the expiry date of a refresh token
                    if (refreshTokenExist.ExpiryDate < DateTime.UtcNow)
                    {
                        return new AuthResult()
                        {
                            Succes = false,
                            Errors = new List<string>()
                            {
                                "Refresh Token has expired, please login again"
                            }
                        };
                    }

                    // Check if Refresh Token has been used or not
                    if (refreshTokenExist.IsUsed)
                    {
                        return new AuthResult()
                        {
                            Succes = false,
                            Errors = new List<string>()
                            {
                                "Refresh Token has been used, It can not be reused."
                            }
                        };
                    }

                    // Check if refresh token has been revoked
                    if (refreshTokenExist.IsRevoked)
                    {
                        return new AuthResult()
                        {
                            Succes = false,
                            Errors = new List<string>()
                            {
                                "Refresh Token has been revoked, It can not be used."
                            }
                        };
                    }

                    // Check if the Refresh Token belongs the the Jwt Id
                    var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                    if (refreshTokenExist.JwtId != jti)
                    {
                        return new AuthResult()
                        {
                            Succes = false,
                            Errors = new List<string>()
                            {
                                "Reference Refresh Token doesn't match the JWT Token.."
                            }
                        };
                    }

                    // Start processing and get a new token
                    refreshTokenExist.IsUsed = true;

                    var updateResult = await _unitOfWork.RefreshTokens.MarkRefreshTokenAsUsed(refreshTokenExist);

                    if (updateResult)
                    {
                        await _unitOfWork.CompleteAsync();

                        // Get the user to generate a new JWT Token
                        var dbUser = await _userManager.FindByIdAsync(refreshTokenExist.UserId);

                        if (dbUser == null)
                        {
                            return new AuthResult()
                            {
                                Succes = false,
                                Errors = new List<string>()
                                {
                                    "Error processing request"
                                }
                            };
                        }

                        // Generate a JWT Token
                        var tokens = await GenerateJwtToken(dbUser);

                        return new AuthResult
                        {
                            Token = tokens.JwtToken,
                            Succes = true,
                            RefreshToken = tokens.RefreshToken
                        };
                    }

                    return new AuthResult()
                    {
                        Succes = false,
                        Errors = new List<string>()
                            {
                                "Error processing request"
                            }
                    };
            }
                catch(Exception ex)
                {
                    // TODO: Add error handling & Logger
                    return null;
                }
        }

        private DateTime UnixTimeStampToDateTime(long unixDate)
        {
            // Sets the date to 1, Jan, 1970
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            // Add the number of seconds from 1 Jan 1970
            dateTime = dateTime.AddSeconds(unixDate).ToUniversalTime();
            return dateTime;
        }

        private async Task<TokenData> GenerateJwtToken(IdentityUser user)
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

            // Generate a refresh token
            var refreshToken = new RefreshToken
            {
                Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
                UserId = user.Id,
                IsRevoked = false,
                IsUsed = false,
                Status = 1,
                JwtId = token.Id,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
            };

            await _unitOfWork.RefreshTokens.Add(refreshToken);
            await _unitOfWork.CompleteAsync();

            var tokenData = new TokenData
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token
            };

            return tokenData;
        }

        private string RandomStringGenerator(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}