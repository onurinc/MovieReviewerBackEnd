using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MR.Api.ControllersV1;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Entities.DTOs.Generic;
using MR.DataAccessLayer.Interfaces;
using System;
using System.Threading.Tasks;

namespace MR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : BaseController
    {
        public UsersController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
        {
        }

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.Users.GetAll();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            var _user = new User();
            _user.FirstName = user.FirstName;
            _user.LastName = user.LastName;
            _user.Email = user.Email;
            _user.Status = 1;

            await _unitOfWork.Users.Add(_user);
            await _unitOfWork.CompleteAsync();
            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _unitOfWork.Users.GetById(id);
            return Ok(user);
        }

    }
}
