using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MR.DataAccessLayer.Context;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MR.DataAccessLayer.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(
            MovieReviewerContext movieReviewerContext, ILogger logger)
            : base(movieReviewerContext, logger)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByMovieId(int movieId)
        {
            try
            {
                return await dbSet.Where(x => x.MovieId == movieId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetReviewsByMovieId method has generated an error", typeof(ReviewRepository));
                return null;
            }
        }

        public override async Task<IEnumerable<Review>> GetAll()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll method has generated an error", typeof(ReviewRepository));
                return new List<Review>();
            }
        }

        public async Task<bool> Upsert(Review review)
        {
            try
            {
                var existingReview =
                    await dbSet.Where(x => x.ReviewId == review.ReviewId)
                    .FirstOrDefaultAsync();

                if (existingReview == null)
                {
                    return false;
                }
                existingReview.Body = review.Body;
                existingReview.Rating = review.Rating;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} UpdateReview method has generated an error", typeof(ReviewRepository));
                return false;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.ReviewId == id)
                    .FirstOrDefaultAsync();

                if (exist != null)
                {
                    dbSet.Remove(exist);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete method error", typeof(ReviewRepository));
                return false;
            }
        }

    }
}
