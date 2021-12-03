using Microsoft.AspNetCore.Mvc;
using MR.Api.ControllersV1;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Entities.Dto;
using MR.DataAccessLayer.Interfaces;
using System;
using System.Threading.Tasks;

namespace MR.Api.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfWork): base(unitOfWork)
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
            _user.DateOfBirth = Convert.ToDateTime(user.DateOfBirth);
            _user.Status = 1;

            await _unitOfWork.Users.Add(_user);
            await _unitOfWork.CompleteAsync();
            return CreatedAtRoute("GetUser", new { id = _user.Id }, user);
        }

        [HttpGet]
        [Route("GetUser", Name = "GetUser")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _unitOfWork.Users.GetById(id);
            return Ok(user);
        }

    }
}
