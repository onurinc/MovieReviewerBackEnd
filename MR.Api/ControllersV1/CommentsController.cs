using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Entities.DTOs.Outgoing;
using MR.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace MR.Api.ControllersV1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : BaseController
    {
        public CommentsController(
    IUnitOfWork unitOfWork,
    UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
        {
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "AppUser")]
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
                _comment.CommentId = Guid.NewGuid();
                _comment.MovieId = comment.MovieId;
                _comment.UserId = identityId;
                _comment.Body = comment.Body;

                await _unitOfWork.Comments.Add(_comment);
                await _unitOfWork.CompleteAsync();

           
                return Ok("Comment has been added");
        }

        [HttpGet("movieid/{movieId}")]
        public async Task<IActionResult> GetCommentByMovieId(int movieId)
        {
            var commentsbyId = await _unitOfWork.Comments.GetCommenstById(movieId);

            if (commentsbyId == null)
            {
                return NotFound();
            }
            return Ok(commentsbyId);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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

            commentToEdit.Body = comment.Body;

            var isUpdated = await _unitOfWork.Comments.Upsert(commentToEdit);

            if (isUpdated)
            {
                await _unitOfWork.CompleteAsync();
                return Ok(comment);
            }

            return BadRequest("Something went wrong, contact the administrator to report the issue");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteItem(Guid id)
        {
            var item = await _unitOfWork.Comments.GetById(id);
            if (item == null)
            {
                return BadRequest();
            }
            await _unitOfWork.Comments.Delete(id);
            await _unitOfWork.CompleteAsync();

            return Ok(id);
        }

    }
}
