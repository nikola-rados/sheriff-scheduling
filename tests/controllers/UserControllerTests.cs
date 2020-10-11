using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS.Api.controllers.usermanagement;
using SS.Api.Models.DB;
using SS.Api.models.dto;
using SS.Api.Models.Dto;
using SS.Api.services;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    /// <summary>
    /// This uses SheriffController which is derived from UserController.
    /// This tests the more general user operations.
    /// </summary>
    public class UserControllerTests : WrapInTransactionScope
    {
        #region Variables
        private readonly SheriffController _controller;
        #endregion Variables

        public UserControllerTests() : base (false)
        {
            _controller = new SheriffController(new SheriffService(_dbContext), new UserService(_dbContext))
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task AssignAndUnassignRoles()
        {
            var user = await CreateUser();
            var role = await CreateRole();

            var controllerResult = await _controller.AssignRoles(new List<AssignRoleDto> {new AssignRoleDto { UserId = user.Id, RoleId = role.Id, EffectiveDate = DateTimeOffset.UtcNow }});
            HttpResponseTest.CheckForNoContentResponse(controllerResult);

            var entity = await _dbContext.User.FindAsync(user.Id);
            Assert.True(entity.UserRoles.Count > 0);

            controllerResult = await _controller.UnassignRoles(new List<UnassignRoleDto> { new UnassignRoleDto { UserId = user.Id, RoleId = role.Id } });
            HttpResponseTest.CheckForNoContentResponse(controllerResult);

            entity = await _dbContext.User.FindAsync(user.Id);
            Assert.True(entity.UserRoles.Count > 0);
            Assert.NotNull(entity.UserRoles.FirstOrDefault().ExpiryDate);
        }


        [Fact]
        public async Task EnableUser()
        {
            var userObject = await CreateUser();

            var controllerResult = await _controller.EnableUser(userObject.Id);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);


            var dbSheriff = await _dbContext.User.FindAsync(userObject.Id);
            Assert.NotNull(dbSheriff);
            Assert.True(dbSheriff.IsEnabled);
        }

        [Fact]
        public async Task DisableUser()
        {
            var userObject = await CreateUser();

            var controllerResult = await _controller.DisableUser(userObject.Id);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            var dbSheriff = await _dbContext.User.FindAsync(response.Id);
            Assert.NotNull(dbSheriff);
            Assert.False(dbSheriff.IsEnabled);
        }

        private async Task<Role> CreateRole()
        {
            var newRole = new Role
            {
                Description = "The big boss",
                Name = "BigBoss"
            };
            await _dbContext.Role.AddAsync(newRole);
            await _dbContext.SaveChangesAsync();
            return newRole;
        }

        private async Task<User> CreateUser()
        {
            var newUser = new User
            {
                FirstName = "Ted",
                LastName = "Tums",
                Email = "Ted@Teddy.com",
                IdirId = new Guid(),
                IdirName = "ted@fakeidir"
            };

            await _dbContext.User.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            return newUser;
        }

    }
}
