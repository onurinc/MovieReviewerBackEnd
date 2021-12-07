using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MR.Api.ControllersV1;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Entities.Dto;
using MR.DataAccessLayer.Interfaces;
using System;
using System.Threading.Tasks;

namespace MR.Api.ControllersV1
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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


    }
}
