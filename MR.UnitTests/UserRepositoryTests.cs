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
    public class UserRepositoryTests : IDisposable
    {

        private readonly IUserRepository _repository;
        private readonly MovieReviewerContext _context;


        public UserRepositoryTests()
        {
            var mocklogger = new Mock<ILogger<UserRepository>>();
            var options = new DbContextOptionsBuilder<MovieReviewerContext>()
                .UseInMemoryDatabase(databaseName: "MockDb")
                .Options;

            _context = new MovieReviewerContext(options);
            this.SeedData(_context);
            _repository = new UserRepository(_context, mocklogger.Object);
        }

        public void SeedData(MovieReviewerContext context)
        {
            context.Users.Add(new User
            {
                IdentityId = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44"),
                Id = new Guid("937b572a-0db3-4ff0-84cc-679034f110fb"),
                FirstName = "Onur",
                MiddleName = null,
                LastName = "Inc",
                Country = "Turkey"
            }
            );

            context.Users.Add(new User
            {
                IdentityId = new Guid("d9419aa6-7b5f-4ad3-8170-0dc12c148b44"),
                Id = new Guid("8a2dba95-ef8f-4e5e-80f2-7856519f504d"),
                FirstName = "Jon",
                MiddleName = null,
                LastName = "Zherka",
                Country = "United States"
            }
            );

            context.SaveChanges();

        }

        public new void Dispose()
        {
            this._context.Database.EnsureDeleted();
        }


        [TestMethod()]
        public async Task GetAllUsers()
        {
            // Arrange

            // Act
            ICollection<User> users = (ICollection<User>)await this._repository.GetAll();
            ICollection<User> usersv2 = (ICollection<User>)await this._repository.GetAll();

            // Assert
            Assert.IsTrue(2 == users.Count);
            Assert.IsTrue(2 == usersv2.Count);
        }

        [TestMethod()]
        public async Task GetUserByIdentityId()
        {
            //Arrange
            Guid UserToSearch = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44");

            //Act
            User user = await this._repository.GetUserByIdentityId(UserToSearch);

            //Assert
            Assert.IsTrue("Onur" == user.FirstName);
            Assert.IsTrue(null == user.MiddleName);
            Assert.IsTrue("Inc" == user.LastName);
            Assert.IsTrue("Turkey" == user.Country);
            Assert.AreEqual(new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44"), user.IdentityId);
        }


        [TestMethod()]
        public async Task GetUserByIdentityId_ReturnsNull_IfUserDoesntExist()
        {
            //Arrange
            Guid UserToSearch = new Guid("ac21c59c-e4e7-43a7-9f21-82a6b83743dd");

            //Act
            User user = await this._repository.GetUserByIdentityId(UserToSearch);

            //Assert

            Assert.AreEqual(null, user);
        }



        [TestMethod()]
        public async Task UpdateUser_ReturnsTrue_IfUserExists()
        {
            //Arrange
            var userToUpdate = new User
            {
                IdentityId = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44"),
                Id = new Guid("937b572a-0db3-4ff0-84cc-679034f110fb"),
                FirstName = "TestUser",
                MiddleName = "TestUser",
                LastName = "TestUser",
                Country = "TestUser"
            };


            //Act

            var isUpdated = await this._repository.UpdateUserProfile(userToUpdate);

            //Assert
            Assert.IsTrue(true == isUpdated);

        }

        [TestMethod()]
        public async Task UpdateUser_ReturnsFalse_IfUserDoesntExist()
        {
            //Arrange
            var userToUpdate = new User
            {
                IdentityId = new Guid("d9419aa6-7a5f-4ad3-8170-0dc12c148b44"),
                Id = new Guid("e5d9b1cf-1844-40ed-ae90-a5caecc7e2c3"),
                FirstName = "TestUser",
                MiddleName = "TestUser",
                LastName = "TestUser",
                Country = "TestUser"
            };


            //Act

            var isUpdated = await this._repository.UpdateUserProfile(userToUpdate);

            //Assert
            Assert.IsTrue(false == isUpdated);

        }

    }
}
