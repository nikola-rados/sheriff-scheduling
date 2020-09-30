using Mapster;
using SS.Api.controllers.usermanagement;
using SS.Api.Models.Dto;
using SS.Api.services;
using SS.Db.models.auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    public class RoleControllerTests : WrapInTransactionScope
    {
        private readonly RoleController _controller;
        public RoleControllerTests() : base(false)
        {
            _controller = new RoleController(new RoleService(_dbContext))
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task GetRoles()
        {
            var role = await CreateRole();

            var controllerResult = await _controller.Roles();
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
       
            Assert.NotNull(response.FirstOrDefault(r => r.Id == role.Id));
        }

        [Fact]
        public async Task AddRole()
        {
            var roleDto = new RoleDto
            {
                Description = "Super Role",
                Name = "Super"
            };

            var controllerResult = await _controller.AddRole(roleDto);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(roleDto.Description, response.Description);
            Assert.Equal(roleDto.Name, response.Name);
            Assert.NotEqual(roleDto.Id, response.Id);
        }
        
        [Fact]
        public async Task UpdateRole()
        {
            var role = await CreateRole();
            role.Name = "hello";
            role.Description = "sugar";

            Detach();

            var controllerResult = await _controller.UpdateRole(role.Adapt<RoleDto>());
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(role.Name, response.Name);
            Assert.Equal(role.Description, response.Description);
            Assert.Equal(role.Id, response.Id);
        }

        [Fact]
        public async Task RemoveRole()
        {
            var role = await CreateRole();

            var controllerResult = await _controller.RemoveRole(role.Id);
            HttpResponseTest.CheckForNoContentResponse(controllerResult);

            Assert.Null(await _dbContext.Role.FindAsync(role.Id));
        }

        [Fact]
        public async Task AssignAndUnassignPermissions()
        {
            var role = await CreateRole();
            var permission = await CreatePermission();

            HttpResponseTest.CheckForNoContentResponse(await _controller.AssignPermissions(role.Id, new List<int> { permission.Id }));

            var savedRole = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.GetRole(role.Id));
            Assert.NotEmpty(savedRole.RolePermissions);

            HttpResponseTest.CheckForNoContentResponse(await _controller.UnassignPermissions(role.Id, new List<int> { permission.Id }));

            var savedRole2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.GetRole(role.Id));
            Assert.Empty(savedRole2.RolePermissions);
        }

        private async Task<Permission> CreatePermission()
        {
            var permission = new Permission {Name = "Good Perm", Description = "hello"};
            await _dbContext.Permission.AddAsync(permission);
            await _dbContext.SaveChangesAsync();
            return permission;
        }

        private async Task<Role> CreateRole()
        {
            var role = new Role {Name = "Super Role"};
            await _dbContext.Role.AddAsync(role);
            await _dbContext.SaveChangesAsync();
            return role;
        }
    }
}
