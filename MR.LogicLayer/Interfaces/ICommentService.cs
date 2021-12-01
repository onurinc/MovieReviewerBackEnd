using MR.LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.LogicLayer.Interfaces
{
    public interface ICommentService
    {
        Task<IList<CommentModel>> GetAllComments();

        Task<CommentModel> GetCommentById(int CommentId);

        Task<CommentModel> CreateComment(CommentModel commentModel);

        Task<CommentModel> UpdateComment(CommentModel commentModel);

        Task DeleteComment(int CommentId);
    }
}
