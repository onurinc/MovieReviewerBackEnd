using Microsoft.Extensions.Logging;
using MR.DataAccessLayer.Context;
using MR.DataAccessLayer.Interfaces;
using MR.DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.DataAccessLayer.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly MovieReviewerContext _movieReviewerContext;
        private readonly ILogger _logger;

        public IUserRepository Users { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public ICommentRepository Comments { get; private set; }

        public UnitOfWork(MovieReviewerContext movieReviewerContext, ILoggerFactory loggerFactory)
        {
            _movieReviewerContext = movieReviewerContext;
            _logger = loggerFactory.CreateLogger("db_logs");

            Users = new UserRepository(movieReviewerContext, _logger);
            Reviews = new ReviewRepository(movieReviewerContext, _logger);
            Comments = new CommentRepository(movieReviewerContext, _logger);
        }

        public async Task CompleteAsync()
        {
            await _movieReviewerContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _movieReviewerContext.Dispose();
        }

    }
}
