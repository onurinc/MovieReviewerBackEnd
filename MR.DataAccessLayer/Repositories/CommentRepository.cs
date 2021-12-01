using Microsoft.EntityFrameworkCore;
using MR.DataAccessLayer.Context;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MR.DataAccessLayer.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly MovieReviewerContext _moviereviewerContext;

        public CommentRepository(MovieReviewerContext moviereviewerContext)
        {
            _moviereviewerContext = moviereviewerContext;
        }


        public async Task<Comment> GetCommentById(int CommentId)
        {
            try
            {
                var item = await _moviereviewerContext.Set<Comment>()
                    .Where(x => x.CommentId == CommentId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (item == null)
                {
                    throw new Exception($"Couldn't find entity with id={CommentId}");
                }

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entity with id={CommentId}: {ex.Message}");
            }
        }

        public async Task<Comment> CreateComment(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            try
            {
                await _moviereviewerContext.Set<Comment>().AddAsync(comment);
                await _moviereviewerContext.SaveChangesAsync();
                return comment;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(comment)} could not be saved: {ex.Message}");
            }
        }

        public async Task<Comment> UpdateComment(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            try
            {
                _moviereviewerContext.Set<Comment>().Update(comment);
                await _moviereviewerContext.SaveChangesAsync();
                return comment;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(comment)} could not be updated: {ex.Message}");
            }
        }

        public async Task<bool> DeleteComment(int CommentId)
        {
            var entity = await _moviereviewerContext.Set<Comment>().FindAsync(CommentId);
            if (entity == null)
            {
                throw new Exception($"{nameof(CommentId)} could not be found.");
            }

            _moviereviewerContext.Set<Comment>().Remove(entity);
            await _moviereviewerContext.SaveChangesAsync();
            return true;
        }

        public Task<IQueryable<Comment>> GetAllComments()
        {
            throw new NotImplementedException();
        }
    }
}
