using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Interfaces;
using MR.LogicLayer.Interfaces;
using MR.LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.LogicLayer.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;


        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<CommentModel> CreateComment(CommentModel commentModel)
        {
            if (commentModel is null)
            {
                throw new ArgumentNullException(nameof(commentModel));
            }

            var commentEntity = new DataAccessLayer.Entities.Comment
            {
                CommentId = commentModel.CommentId,
                UserId = commentModel.UserId,
                MovieId = commentModel.MovieId,
                Body = commentModel.Body,
            };

            commentEntity = await _commentRepository.CreateComment(commentEntity);

            return new CommentModel
            {
                CommentId = commentEntity.CommentId,
                UserId = commentEntity.UserId,
                MovieId = commentEntity.MovieId,
                Body = commentEntity.Body,
            };
        }

        public async Task DeleteComment(int CommentId)
        {
            await _commentRepository.DeleteComment(CommentId);
        }


        public async Task<CommentModel> GetCommentById(int CommentId)
        {
            var commentEntity = await _commentRepository.GetCommentById(CommentId);

            if (commentEntity is null)
            {
                return null;
            }

            return new CommentModel
            {
                CommentId = commentEntity.CommentId,
                UserId = commentEntity.UserId,
                MovieId = commentEntity.MovieId,
                Body = commentEntity.Body,
            };
        }

        public async Task<CommentModel> UpdateComment(CommentModel commentModel)
        {
            var commentEntity = new MR.DataAccessLayer.Entities.Comment
            {
                CommentId = commentModel.CommentId,
                UserId = commentModel.UserId,
                MovieId = commentModel.MovieId,
                Body = commentModel.Body,
            };

            commentEntity = await _commentRepository.UpdateComment(commentEntity);

            return new CommentModel
            {
                CommentId = commentEntity.CommentId,
                UserId = commentEntity.UserId,
                MovieId = commentEntity.MovieId,
                Body = commentEntity.Body,
            };
        }

        Task<IList<CommentModel>> ICommentService.GetAllComments()
        {
            throw new NotImplementedException();
        }
    }
}
