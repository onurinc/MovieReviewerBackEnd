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
    public class ReviewRepositoryTests : IDisposable
    {
        private readonly IReviewRepository _repository;
        private readonly MovieReviewerContext _context;


        public ReviewRepositoryTests()
        {
            var mocklogger = new Mock<ILogger<ReviewRepository>>();
            var options = new DbContextOptionsBuilder<MovieReviewerContext>()
                .UseInMemoryDatabase(databaseName: "MockDb")
                .Options;

            _context = new MovieReviewerContext(options);
            this.SeedData(_context);
            _repository = new ReviewRepository(_context, mocklogger.Object);
        }

        public void SeedData(MovieReviewerContext context)
        {
            context.Reviews.Add(new Review
            {
                ReviewId = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44"),
                UserId = new Guid(),
                MovieId = 1,
                Body = "Test review one",
                Rating = 10
            }
            );

            context.Reviews.Add(new Review
            {
                ReviewId = new Guid(),
                UserId = new Guid(),
                MovieId = 1,
                Body = "Test review two",
                Rating = 10
            }
            );

            context.SaveChanges();

        }

        public new void Dispose()
        {
            this._context.Database.EnsureDeleted();
        }

        [TestMethod()]
        public async Task GetAllReviews()
        {
            // Arrange

            // Act
            ICollection<Review> reviews = (ICollection<Review>)await this._repository.GetAll();
            ICollection<Review> reviewsv2 = (ICollection<Review>)await this._repository.GetAll();

            // Assert
            Assert.IsTrue(2 == reviews.Count);
            Assert.IsTrue(2 == reviewsv2.Count);
        }

        [TestMethod()]
        public async Task GetRevieByGuid()
        {
            //Arrange
            Guid ReviewIdToSearch = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44");

            //Act
            Review review = await this._repository.GetById(ReviewIdToSearch);

            //Assert
            Assert.IsTrue("Test review one" == review.Body);
            Assert.AreEqual(1, review.MovieId);
        }

        [TestMethod()]
        public async Task CreateReviewAsync()
        {
            Review newReview = new Review
            {
                ReviewId = new Guid("92c52676-49e5-403f-9938-075b1edeca0a"),
                UserId = new Guid(),
                MovieId = 2,
                Body = "Test review three",
                Rating = 8
            };

            await this._repository.Add(newReview);

            Guid reviewToSearch = new Guid("92c52676-49e5-403f-9938-075b1edeca0a");

            Review review = await this._repository.GetById(reviewToSearch);

            Assert.IsTrue(2 == newReview.MovieId);
            Assert.IsTrue(2 == review.MovieId);
            Assert.IsTrue(8 == review.Rating);

        }

        [TestMethod()]
        public async Task UpdateReviewAsync()
        {
            Guid ReviewToSearch = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44");
            Review review = await this._repository.GetById(ReviewToSearch);

            review.Body = "Edited body";
            await this._repository.Upsert(review);

            Review updatedReview = await this._repository.GetById(ReviewToSearch);

            Assert.IsTrue("Edited body" == review.Body);
            Assert.IsTrue("Edited body" == updatedReview.Body);
        }

    }
}
