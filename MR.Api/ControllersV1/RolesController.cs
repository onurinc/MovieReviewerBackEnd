﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MR.Api.ControllersV1;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Entities.DTOs.Generic;
using MR.DataAccessLayer.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MR.Api.ControllersV1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ILogger<RolesController> _logger;

        public RolesController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RolesController> logger) : base(unitOfWork, userManager)
        {
            _roleManager = roleManager;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            // Check if the role exist
            var roleExist = await _roleManager.RoleExistsAsync(name);

            if (!roleExist) // checks on the role exist status
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(name));

                // We need to check if the role has been added successfully
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"The Role {name} has been added successfully");
                    return Ok(new
                    {
                        result = $"The role {name} has been added successfully"
                    });
                }
                else
                {
                    _logger.LogInformation($"The Role {name} has not been added");
                    return BadRequest(new
                    {
                        error = $"The role {name} has not been added"
                    });
                }

            }

            return BadRequest(new { error = "Role already exist" });
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            // Check if the user exist
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // User does not exist
            {
                _logger.LogInformation($"The user with the {email} does not exist");
                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            // Check if the role exist
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist) // checks on the role exist status
            {
                _logger.LogInformation($"The role {email} does not exist");
                return BadRequest(new
                {
                    error = "Role does not exist"
                });
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            // Check if the user is assigned to the role successfully
            if (result.Succeeded)
            {
                return Ok(new
                {
                    result = "Success, user has been added to the role"
                })
                ;
            }
            else
            {
                _logger.LogInformation($"The user was not able to be added to the role");
                return BadRequest(new
                {
                    error = "The user was not able to be added to the role"
                });
            }
        }

        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            // check if the email is valid
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // User does not exist
            {
                _logger.LogInformation($"The user with the {email} does not exist");
                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            // return the roles
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }

        [HttpPost]
        [Route("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            // Check if the user exist
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // User does not exist
            {
                _logger.LogInformation($"The user with the {email} does not exist");
                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            // Check if the role exist
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist) // checks on the role exist status
            {
                _logger.LogInformation($"The role {email} does not exist");
                return BadRequest(new
                {
                    error = "Role does not exist"
                });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    result = $"User {email} has been removed from role {roleName}"
                });
            }

            return BadRequest(new
            {
                error = $"Unable to remove User {email} from role {roleName}"
            });

        }
    }
}   
