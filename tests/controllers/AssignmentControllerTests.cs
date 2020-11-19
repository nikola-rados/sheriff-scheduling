using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.Api.controllers.scheduling;
using SS.Api.Models.DB;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using ss.db.models;
using SS.Db.models.scheduling;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    //Sequential, because there are issues with Adding Location (with a unique index) within a TransactionScope.
    [Collection("Sequential")]
    public class AssignmentControllerTests : WrapInTransactionScope
    {
        #region Fields
        private readonly AssignmentController _controller;
        #endregion Fields

        public AssignmentControllerTests() : base(false)
        {
            _controller = new AssignmentController(new AssignmentService(Db), Db)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }
        [Fact]
        public async Task TestExpireAssignment()
        {
            var assignment = new Assignment
            {
                LookupCode =  new LookupCode { Id = 5000000 },
                Location = new Location {Id = 1, AgencyId = "44"},
                Monday = true,
                Tuesday = true,
                Wednesday = true,
                Thursday = true,
                Friday = true,
                Saturday = true,
                Sunday = true,
                LocationId = 1,
                AdhocStartDate = DateTimeOffset.UtcNow.AddDays(-1),
                AdhocEndDate = DateTimeOffset.UtcNow.AddDays(7),
                Timezone = "America/Edmonton"
            };

            await Db.Assignment.AddAsync(assignment);
            await Db.SaveChangesAsync();

            var duty = new Duty
            {
                AssignmentId = assignment.Id,
                ExpiryDate = null,
                LocationId = 1,
                StartDate = DateTimeOffset.UtcNow.AddDays(1),
                DutySlots = new List<DutySlot>
                {
                    new DutySlot
                    {
                        LocationId = 1,
                        ExpiryDate = null
                    }
                }
            };

            var duty2 = new Duty
            {
                AssignmentId = assignment.Id,
                ExpiryDate = null,
                LocationId = 1,
                StartDate = DateTimeOffset.UtcNow,
                DutySlots = new List<DutySlot>
                    {
                        new DutySlot
                        {
                            LocationId = 1,
                            ExpiryDate = null
                        }
                    }
            };

            await Db.Duty.AddAsync(duty);

            await Db.Duty.AddAsync(duty);
            await Db.SaveChangesAsync();

            var controllerResult3 = await _controller.GetAssignments(1, DateTimeOffset.UtcNow, assignment.AdhocEndDate);
            var hasCount1 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);
            Assert.NotEmpty(hasCount1);

            HttpResponseTest.CheckForNoContentResponse(await _controller.ExpireAssignment(assignment.Id, "EXPIRED"));
            var controllerResult = await _controller.GetAssignments(1, DateTimeOffset.UtcNow, assignment.AdhocEndDate);
            var zeroCount = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.Empty(zeroCount);

            var controllerResult2 = await _controller.GetAssignments(1, assignment.AdhocStartDate.Value.AddDays(-1), assignment.AdhocEndDate);
            var hasCount2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);
            Assert.NotEmpty(hasCount2);
        }

    }
}
