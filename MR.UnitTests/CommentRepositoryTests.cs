using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MR.DataAccessLayer.Context;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Interfaces;
using MR.DataAccessLayer.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework.Internal;

namespace MR.UnitTests
{
    [TestClass()]
    public class CommentRepositoryTests : IDisposable
    {
        private readonly ICommentRepository _repository;
        private readonly MovieReviewerContext _context;


        public CommentRepositoryTests()
        {
            var mocklogger = new Mock<ILogger<CommentRepository>>();
            var options = new DbContextOptionsBuilder<MovieReviewerContext>()
                .UseInMemoryDatabase(databaseName: "MockDb")
                .Options;

            _context = new MovieReviewerContext(options);
            this.SeedData(_context);
            _repository = new CommentRepository(_context, mocklogger.Object);
        }

        public void SeedData(MovieReviewerContext context)
        {
            context.Comments.Add(new Comment
            {
                CommentId = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44"),
                UserId = new Guid(),
                MovieId = 1,
                Body = "Test comment"
            }
            );
            context.Comments.Add(new Comment
            {
                CommentId = new Guid(),
                UserId = new Guid(),
                MovieId = 1,
                Body = "Test comment two"
            }
);

            context.SaveChanges();

        }

        public new void Dispose()
        {
            this._context.Database.EnsureDeleted();
        }


        [TestMethod()]
        public async Task GetAllComments()
        {
            // Arrange

            // Act
            ICollection<Comment> comments = (ICollection<Comment>)await this._repository.GetAll();
            ICollection<Comment> commentsv2 = (ICollection<Comment>)await this._repository.GetAll();

            // Assert
            Assert.IsTrue(2 == comments.Count);
            Assert.IsTrue(2 == commentsv2.Count);
        }

        [TestMethod()]
        public async Task GetCommentByGuid()
        {
            //Arrange
            Guid CommentIdToSearch = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44");

            //Act
            Comment comment = await this._repository.GetById(CommentIdToSearch);

            //Assert
            Assert.IsTrue("Test comment" == comment.Body);
            Assert.AreEqual(1, comment.MovieId);
        }

        [TestMethod()]
        public async Task GetCommentByGuid_ReturnsNull_IfCommentDoesntExist()
        {
            //Arrange
            Guid CommentIdToSearch = new Guid("d9419aa6-7b5f-4ad3-8170-0dc12c148b44");

            //Act
            Comment comment = await this._repository.GetById(CommentIdToSearch);

            //Assert
            Assert.AreEqual(null, comment);
        }

        [TestMethod()]
        public async Task CreateCommentAsync()
        {
            Comment newComment = new Comment
            {
                CommentId = new Guid("92c52676-49e5-403f-9938-075b1edeca0a"),
                UserId = new Guid(),
                MovieId = 2,
                Body = "Test comment two"
            };

            await this._repository.Add(newComment);

            Guid CommentIdToSearch = new Guid("92c52676-49e5-403f-9938-075b1edeca0a");

            Comment comment = await this._repository.GetById(CommentIdToSearch);

            Assert.IsTrue(2 == newComment.MovieId);
            Assert.IsTrue(2 == comment.MovieId);
        }

        [TestMethod()]
        public async Task UpdateCommentAsync()
        {
            Guid CommentIdToSearch = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44");
            Comment comment = await this._repository.GetById(CommentIdToSearch);

            comment.Body = "Edited body";
            await this._repository.Upsert(comment);

            Comment updatedComment = await this._repository.GetById(CommentIdToSearch);

            Assert.IsTrue("Edited body" == comment.Body);
            Assert.IsTrue("Edited body" == updatedComment.Body);
        }

        [TestMethod()]
        public async Task DeleteComment_ReturnsTrue_IfCommentExists()
        {
            Guid CommentIdToSearch = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44");
            bool outcome = await this._repository.Delete(CommentIdToSearch);

            Assert.AreEqual(outcome, true);
        }

        [TestMethod()]
        public async Task DeleteComment_ReturnsFalse_IfCommentDoesntExists()
        {
            Guid CommentIdToSearch = new Guid("1e430f9b-2dd1-4c90-af4a-94c12f3321e3");
            bool outcome = await this._repository.Delete(CommentIdToSearch);

            Assert.AreEqual(outcome, false);
        }

    }
}
