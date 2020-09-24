using Microsoft.EntityFrameworkCore;
using SS.Api.models.db;
using SS.Api.Models.Dto;
using SS.Api.services;
using System;
using System.Threading.Tasks;
using SS.Api.controllers;
using SS.Db.models;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    /// <summary>
    /// I've created one test here, because the manage types are very similar in controller / service structure. 
    /// </summary>
    public class ManageTypesControllerTests : WrapInTransactionScope
    {
        #region Variables

        private readonly ManageTypesController _controller;
        private SheriffDbContext _dbContext;
        #endregion Variables

        public ManageTypesControllerTests()
        {
            _dbContext = new SheriffDbContext(EnvironmentBuilder.SetupDbOptions(useMemoryDatabase:true));
            _controller = new ManageTypesController(new ManageTypesService(_dbContext))
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task AddLookup()
        {
            var courtRole = new LookupCodeDto
            {
                Type = LookupTypes.CourtRole
            };
            var controllerResult = await _controller.Add(courtRole);
            var response = HttpResponseTest.CheckForValidHttpResponseAndReturnValue(controllerResult);
            Assert.True(response.Id > 0);
        }

        [Fact]
        public async Task GetLookupCodes()
        {
            await AddCourtRole();
            var controllerResult = await _controller.GetAll(LookupTypes.CourtRole, 5);
            var response = HttpResponseTest.CheckForValidHttpResponseAndReturnValue(controllerResult);
            Assert.True(response.Count >= 1);

            var controllerResult2 = await _controller.GetAll(LookupTypes.EscortRun, 5);
            var response2 = HttpResponseTest.CheckForValidHttpResponseAndReturnValue(controllerResult2);
            Assert.True(response2.Count == 0);
        }

        [Fact]
        public async Task UpdateLookupCode()
        {
            var id = await AddCourtRole();
            var result = HttpResponseTest.CheckForValidHttpResponseAndReturnValue(await _controller.Find(id));

            Detach(); //We get an exception if we don't detach before the update. 

            var createdById = new Guid();
            result.CreatedById = createdById;
            result.CreatedOn = DateTime.Now;
            result.Description = "test";
            result.EffectiveDate = DateTime.Now;
            result.Code = "gg";
            result.SubCode = "gg2";
            result.ExpiryDate = DateTime.Now;
            result.SortOrder = 5;
            result.Type = LookupTypes.JailRole;
            var updatedById = new Guid();
            result.UpdatedById = updatedById;
            result.UpdatedOn = DateTime.Now;
       
            result.LocationId = 6;

            var controllerResult = await _controller.Update(result);
            HttpResponseTest.CheckForValidHttpResponseAndReturnValue(controllerResult);

            result = HttpResponseTest.CheckForValidHttpResponseAndReturnValue(await _controller.Find(id));

            Assert.Equal(createdById, result.CreatedById);
            Assert.Equal(DateTime.Now, result.CreatedOn, TimeSpan.FromSeconds(10));
            Assert.Equal("test", result.Description);
            Assert.Equal(DateTime.Now, result.EffectiveDate.Value, TimeSpan.FromSeconds(10));
            Assert.Equal(DateTime.Now, result.ExpiryDate.Value, TimeSpan.FromSeconds(10));
            Assert.Equal(5, result.SortOrder);
            Assert.Equal(LookupTypes.JailRole, result.Type);
            Assert.Equal(updatedById, result.UpdatedById);
            Assert.Equal(DateTime.Now, result.UpdatedOn.Value, TimeSpan.FromSeconds(10));
            Assert.Equal("gg", result.Code);
            Assert.Equal("gg2", result.SubCode);
        }

        [Fact]
        public async Task RemoveLookupCode()
        {
            var id = await AddCourtRole();

            var controllerResult2 = await _controller.Remove(id);
            HttpResponseTest.CheckForNoContentResponse(controllerResult2);

            var controllerResult3 = await _controller.Find(id);
            HttpResponseTest.CheckForNotFound(controllerResult3);
        }

        private async Task<int> AddCourtRole()
        {
            var courtRole = new LookupCodeDto
            {
                Description = "test",
                Type = LookupTypes.CourtRole,
                LocationId = 5
            };
            var controllerResult = await _controller.Add(courtRole);
            var result = HttpResponseTest.CheckForValidHttpResponseAndReturnValue(controllerResult);
            return result.Id;
        }

        #region Helpers
        private void Detach()
        {
            foreach (var entity in _dbContext.ChangeTracker.Entries())
                entity.State = EntityState.Detached;
        }
        #endregion Helpers
    }
}
