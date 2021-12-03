using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Interfaces;
using MR.LogicLayer.Interfaces;
using MR.LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MR.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("{CommentId}")]
        [ActionName("GetTodoAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentModel>> GetCommentById(int CommentId)
        {
            var comment = await _commentService.GetCommentById(CommentId);

            if (comment is null)
            {
                return NotFound();
            }

            return Ok(comment);
        }


        // TODO: Get functie fixen in de hele project
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<List<CommentModel>>> GetAllComments()
        //{
        //    var comments = await _commentService.GetAllComments();
        //    return Ok(comments);
        //}

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<CommentModel>> CreateComment(CommentModel createcommentModel)
        {
            var commentModel = new CommentModel
            {
                CommentId = createcommentModel.CommentId,
                UserId = createcommentModel.UserId,
                MovieId = createcommentModel.MovieId,
                Body = createcommentModel.Body,
            }; 

            await _commentService.CreateComment(commentModel);
            return Ok();
        }

        [HttpPut("{commentId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateTodoAsync(int commentId, CommentModel updatecommentModel)
        {
            if (commentId != updatecommentModel.CommentId)
            {
                return BadRequest();
            }

            var commentbyid = await _commentService.GetCommentById(commentId);
            if (commentbyid is null)
            {
                return NotFound();
            }

            var commentModel = new CommentModel
            {
                CommentId = commentId,
                UserId = updatecommentModel.UserId,
                MovieId = updatecommentModel.MovieId,
                Body = updatecommentModel.Body,
            };

            var updatedTodo = await _commentService.UpdateComment(commentModel);
            return Ok();
        }

        [HttpDelete("{commentId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteCommentById(int commentId)
        {
            var comment = await _commentService.GetCommentById(commentId);
            if (comment is null)
            {
                return NotFound();
            }

            await _commentService.DeleteComment(commentId);
            return NoContent();
        }

    }
}
