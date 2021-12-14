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
    class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(
            MovieReviewerContext movieReviewerContext, ILogger logger)
            : base(movieReviewerContext, logger)
        {
        }

        public override async Task<IEnumerable<Review>> GetAll()
        {
            try
            {
                return await dbSet.Where(x => x.Status == 1).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll method has generated an error", typeof(ReviewRepository));
                return new List<Review>();
            }
        }

        public async Task<bool> UpdateReview(Review review)
        {
            try
            {
                var existingReview =
                    await dbSet.Where(x => x.Status == 1
                    && x.Id == review.Id)
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

    }
}
