using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MR.DataAccessLayer.Interfaces;
using MR.DataAccessLayer.Entities.DTOs.Incoming;
using System;
using System.Threading.Tasks;

namespace MR.Api.ControllersV1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : BaseController
    {
        public ProfileController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
            {
            // Based on the JWT Token a User obj is attached to this. This allows us to get the LoggedInUser
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                return BadRequest("User not found");
            }

            var identityId = new Guid(loggedInUser.Id);
           
            var profile = await _unitOfWork.Users.GetUserByIdentityId(identityId);

            if (profile == null)
            {
                return BadRequest("User not found");
            }

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid payload");
            }

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
                            
                if (loggedInUser == null)
            {
                return BadRequest("User not found");
            }

            var identityId = new Guid(loggedInUser.Id);

            var userProfile = await _unitOfWork.Users.GetUserByIdentityId(identityId);

            if (userProfile == null)
            {
                return BadRequest("User not found");
            }

            userProfile.FirstName = updateProfileDto.FirstName;
            userProfile.MiddleName = updateProfileDto.MiddleName;
            userProfile.LastName = updateProfileDto.LastName;
            userProfile.Country = updateProfileDto.Country;

            var isUpdated = await _unitOfWork.Users.UpdateUserProfile(userProfile);

            if (isUpdated)
            {
                await _unitOfWork.CompleteAsync();
                return Ok(userProfile);
            }

            return BadRequest("Something went wrong, contact the administrator to report the issue");

        }

    }
}
