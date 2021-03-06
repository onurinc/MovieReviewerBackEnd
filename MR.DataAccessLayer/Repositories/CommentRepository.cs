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
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {

        public CommentRepository(
            MovieReviewerContext movieReviewerContext, ILogger logger)
            : base(movieReviewerContext, logger)
        { 
        }

        public override async Task<IEnumerable<Comment>> GetAll()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll method error", typeof(CommentRepository));
                return new List<Comment>();
            }
        }

        public async Task<IEnumerable<Comment>> GetCommenstById(int movieId)
        {
            try
            {
                return await dbSet.Where(x => x.MovieId == movieId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetCommentsById method has generated an error", typeof(CommentRepository));
                return null;
            }
        }

        public override async Task<bool> Upsert(Comment comment)
        {
            try
            {
                var existingComment = await dbSet.Where(x => x.CommentId == comment.CommentId)
                    .FirstOrDefaultAsync();

                if (existingComment != null)
                existingComment.Body = comment.Body;

                return true;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert method error", typeof(UserRepository));
                return false;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.CommentId == id)
                    .FirstOrDefaultAsync();

                if(exist != null)
                {
                    dbSet.Remove(exist);
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete method error", typeof(UserRepository));
                return false;
            }
        }
    }
}
