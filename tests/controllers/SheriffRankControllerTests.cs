using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SS.Api.controllers.usermanagement;
using SS.Api.models.dto.generated;
using SS.Api.Models.DB;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Db.models.sheriff;
using System;
using System.Linq;
using System.Threading.Tasks;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    public class SheriffRankControllerTests : WrapInTransactionScope
    {
        #region Variables
        private readonly SheriffRankController _controller;
        #endregion

        public SheriffRankControllerTests() : base(false)
        {
            var environment = new EnvironmentBuilder("LocationServicesClient:Username", "LocationServicesClient:Password", "LocationServicesClient:Url");
            var httpContextAccessor = new HttpContextAccessor { HttpContext = HttpResponseTest.SetupHttpContext() };

            var sheriffService = new SheriffService(Db, environment.Configuration, httpContextAccessor);
            var sheriffRankService = new SheriffRankService(Db);
            var shiftService = new ShiftService(Db, sheriffService, environment.Configuration);
            var dutyRosterService = new DutyRosterService(Db, environment.Configuration, shiftService, environment.LogFactory.CreateLogger<DutyRosterService>());
            _controller = new SheriffRankController(sheriffService, sheriffRankService, new UserService(Db), Db, shiftService, dutyRosterService)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        #region Tests
        [Fact]
        public async Task SheriffAssignAndUpdateRankAsync()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var sheriffRanks = Db.SheriffRank.Where(sr => sr.SheriffId == sheriffObject.Id);
            Db.SheriffRank.RemoveRange(sheriffRanks);

            var addRank = new AddSheriffRankDto()
            {
                Rank = "Best",
                SheriffId = sheriffObject.Id,
            };
            var assignedRank = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AssignRank(addRank));

            Assert.Equal("Best", assignedRank.Rank);

            var updateRank = assignedRank.Adapt<UpdateSheriffRankDto>();
            updateRank.Rank = "Super";

            var updatedRank = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.UpdateRank(updateRank));
            Assert.Equal("Super", updatedRank.Rank);
        }
        #endregion

        private async Task<Sheriff> CreateSheriffUsingDbContext()
        {
            var newSheriff = new Sheriff
            {
                FirstName = "Ted",
                LastName = "Tums",
                BadgeNumber = "555",
                Email = "Ted@Teddy.com",
                Gender = Gender.Female,
                IdirId = new Guid(),
                IdirName = "ted@fakeidir",
                HomeLocation = new Location { Name = "Teds Place", AgencyId = "5555555435353535353535353" },
            };

            await Db.Sheriff.AddAsync(newSheriff);
            await Db.SaveChangesAsync();

            Detach();

            return newSheriff;
        }
    }
}
