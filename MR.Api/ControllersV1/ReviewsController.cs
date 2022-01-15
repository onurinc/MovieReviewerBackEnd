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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : BaseController
    {

        public ReviewsController(
    IUnitOfWork unitOfWork,
    UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
        {
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "AppUser")]
        [HttpPost]
        public async Task<IActionResult> CreateReview(Review review)
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

            var _review = new Review();
            _review.ReviewId = Guid.NewGuid();
            _review.UserId = identityId;
            _review.MovieId = review.MovieId;
            _review.Body = review.Body;
            _review.Rating = review.Rating;

            await _unitOfWork.Reviews.Add(_review);
            await _unitOfWork.CompleteAsync();

            return Ok("Review has been added");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(Guid id)
        {
            var review = await _unitOfWork.Reviews.GetById(id);

            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _unitOfWork.Reviews.GetAll();

            if (reviews == null)
            {
                return NotFound();
            }
            return Ok(reviews);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(Guid id, Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid payload");
            }

            var reviewToEdit = await _unitOfWork.Reviews.GetById(id);

            if (reviewToEdit == null)
            {
                return BadRequest("Review not found");
            }

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                return BadRequest("You need to be logged in to perform this action");
            }

            reviewToEdit.Body = review.Body;
            reviewToEdit.Rating = review.Rating;

            var isUpdated = await _unitOfWork.Reviews.Upsert(reviewToEdit);

            if (isUpdated)
            {
                await _unitOfWork.CompleteAsync();
                return Ok(review);
            }

            return BadRequest("Something went wrong, contact the administrator to report the issue");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _unitOfWork.Reviews.GetById(id);
            if (item == null)
            {
                return BadRequest();
            }
            await _unitOfWork.Reviews.Delete(id);
            await _unitOfWork.CompleteAsync();

            return Ok(id);
        }
    }
}
