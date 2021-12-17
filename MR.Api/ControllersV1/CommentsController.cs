using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MR.Api.ControllersV1;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Entities.DTOs.Generic;
using MR.DataAccessLayer.Entities.DTOs.Outgoing;
using MR.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace MR.Api.ControllersV1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : BaseController
    {
        public CommentsController(
    IUnitOfWork unitOfWork,
    UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResult()
                {
                    Succes = false,
                    Errors = new List<string>()
                        {
                            "Something went wrong"
                        }
                });
            }

                var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

                if (loggedInUser == null)
                {
                    return BadRequest("You need to be logged in to perform this task");
                }

                var identityId = new Guid(loggedInUser.Id);

                var _comment = new Comment();
                _comment.CommentId = new Guid();
                _comment.MovieId = 1;
                _comment.UserId = identityId;
                _comment.Body = comment.Body;

                await _unitOfWork.Comments.Add(_comment);
                await _unitOfWork.CompleteAsync();

                return Ok(comment);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(Guid id)
        {
            var comment = await _unitOfWork.Comments.GetById(id);

            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            var users = await _unitOfWork.Comments.GetAll();

            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid payload");
            }

            var commentToEdit = await _unitOfWork.Comments.GetById(id);

            if(commentToEdit == null)
            {
                return BadRequest("Comment not found");
            }

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                return BadRequest("You need to be logged in to perform this action");
            }

            var identityId = new Guid(loggedInUser.Id);

            commentToEdit.CommentId = comment.CommentId;
            commentToEdit.MovieId = comment.MovieId;
            commentToEdit.UserId = identityId;
            commentToEdit.Body = comment.Body;

            var isUpdated = await _unitOfWork.Comments.Upsert(commentToEdit);

            if (isUpdated)
            {
                await _unitOfWork.CompleteAsync();
                return Ok(comment);
            }

            return BadRequest("Something went wrong, contact the administrator to report the issue");
        }
    }
}
