using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using SS.Api.controllers.scheduling;
using SS.Api.controllers.usermanagement;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.exceptions;
using SS.Api.Models.DB;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Db.models.scheduling;
using SS.Db.models.sheriff;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    public class ShiftControllerTests : WrapInTransactionScope
    {
        private ShiftController ShiftController { get; }
        public ShiftControllerTests() : base(false)
        {
            ShiftController = new ShiftController(new ShiftService(Db), new SheriffService(Db))
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task GetShifts()
        {
            var edmontonTz = DateTimeZoneProviders.Tzdb["America/Edmonton"];
            var startDateNowEdmonton = SystemClock.Instance.GetCurrentInstant().InZone(edmontonTz);
            var endDateNowEdmonton = SystemClock.Instance.GetCurrentInstant().InZone(edmontonTz).PlusHours(24*5);

            var startTimeOffset = startDateNowEdmonton.ToDateTimeOffset();
            var endTimeOffset = endDateNowEdmonton.ToDateTimeOffset();

            await Db.Shift.AddAsync(new Shift
            {
                Id = 1, 
                Type = ShiftType.Courts,
                StartDate = startTimeOffset,
                EndDate = endTimeOffset,
                Sheriff = new Sheriff { Id = Guid.NewGuid(), LastName = "hello" },
                AnticipatedAssignment = new Assignment {Id = 1, Name = "Super assignment", Location = new Location { Id = 1, AgencyId = "zz"}},
                LocationId = 1
            });

            await Db.SaveChangesAsync();

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.GetShifts(1, startTimeOffset, endTimeOffset));
            Assert.NotEmpty(response);
            Assert.NotNull(response[0].Sheriff);
            Assert.NotNull(response[0].AnticipatedAssignment);
            Assert.NotNull(response[0].Location);
        }

        [Fact]
        public async Task AddShift()
        {
            var shiftDto = await CreateShift();
            var shift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShift(shiftDto));
        }

        [Fact]
        public async Task UpdateShift()
        {
            var shiftDto = await CreateShift();
            var sheriffId = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff { Id = sheriffId, FirstName = "Hello", LastName = "There"});
            await Db.Assignment.AddAsync(new Assignment { Id = 5, LocationId = 1 });
            await Db.SaveChangesAsync();

            var shift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShift(shiftDto));
            shift.Type = ShiftType.Escorts;
            shift.StartDate = DateTimeOffset.UtcNow.AddDays(5);
            shift.EndDate = DateTimeOffset.UtcNow.AddDays(6);
            shift.LocationId = 5; // This shouldn't change 
            shift.ExpiryDate = DateTimeOffset.UtcNow; // this shouldn't change
            shift.SheriffId = sheriffId;
            shift.Sheriff = new SheriffDto(); // shouldn't change
            shift.Duties = new List<DutyDto>(); // shouldn't change
            shift.AnticipatedAssignment = new AssignmentDto(); //this shouldn't create new. 
            shift.AnticipatedAssignmentId = 5;
            shift.Location = new LocationDto(); // shouldn't change
            shift.LocationId = 5555; // shouldn't change

            var updatedShift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.UpdateShift(shift));

            Assert.Equal(shiftDto.LocationId,updatedShift.LocationId);
            Assert.Null(updatedShift.ExpiryDate);
            Assert.Equal(5, updatedShift.AnticipatedAssignmentId);
            Assert.Equal(sheriffId, updatedShift.SheriffId);

            //Create the same shift, without sheriff, should conflict.
            shiftDto.SheriffId = null;
            shiftDto.StartDate = DateTimeOffset.UtcNow.AddDays(5);
            shiftDto.EndDate = DateTimeOffset.UtcNow.AddDays(6);
            shift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShift(shiftDto));

            shift.SheriffId = sheriffId;
            await Assert.ThrowsAsync<BusinessLayerException>(() => ShiftController.UpdateShift(shift));
        }

        [Fact]
        public async Task AssignToShift()
        {
            var shifts = new List<int> {1, 2};
            var sheriffId = Guid.NewGuid();
            await Db.Location.AddAsync(new Location { Id = 1, AgencyId = "zz" });
            await Db.Sheriff.AddAsync(new Sheriff { Id = sheriffId });

            //Two shifts no conflicts.
            await Db.Shift.AddAsync(new Shift { Id = 1, StartDate = DateTimeOffset.UtcNow, EndDate = DateTimeOffset.UtcNow.AddHours(2), LocationId = 1 });
            await Db.Shift.AddAsync(new Shift { Id = 2, StartDate = DateTimeOffset.UtcNow.AddHours(1), EndDate = DateTimeOffset.UtcNow.AddHours(2), LocationId = 1 });
            await Db.SaveChangesAsync();

            Detach();

            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await ShiftController.AssignToShifts(shifts, sheriffId, false));

            //Two new shifts, should conflict now. 
            shifts = new List<int> { 3, 4 };

            await Db.Shift.AddAsync(new Shift { Id = 3, StartDate = DateTimeOffset.UtcNow, EndDate = DateTimeOffset.UtcNow.AddHours(2), LocationId = 1 });
            await Db.Shift.AddAsync(new Shift { Id = 4, StartDate = DateTimeOffset.UtcNow.AddHours(1), EndDate = DateTimeOffset.UtcNow.AddHours(2), LocationId = 1 });
            await Db.SaveChangesAsync();

            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await ShiftController.AssignToShifts(shifts, sheriffId, false));
        }

        [Fact]
        public async Task RemoveShift()
        {
            var shiftDto = await CreateShift();
            var shift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShift(shiftDto));

            HttpResponseTest.CheckForNoContentResponse(await ShiftController.RemoveShift(shift.Id));

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.GetShifts(1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(5)));
            Assert.Empty(response);
        }

        [Fact]
        public async Task ImportWeeklyShifts()
        {
            var sheriffId = Guid.NewGuid();
            var shiftDto = await CreateShift();
            await Db.Sheriff.AddAsync(new Sheriff {Id = sheriffId});
            shiftDto.StartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6); //Last week monday
            shiftDto.EndDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 5); //Last week tuesday
            shiftDto.SheriffId = sheriffId;

            var shift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShift(shiftDto));

            var importedShifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await ShiftController.ImportWeeklyShifts(1, true));

            Assert.NotNull(importedShifts);
            var importedShift = importedShifts.First();
            Assert.Equal(shiftDto.StartDate.AddDays(7),importedShift.StartDate); //This week monday
            Assert.Equal(shiftDto.EndDate.AddDays(7), importedShift.EndDate); //This week monday
            Assert.Equal(shiftDto.SheriffId, importedShift.SheriffId);
        }

        [Fact]
        public async Task GetAvailability()
        {
            var edmontonTz = DateTimeZoneProviders.Tzdb["America/Edmonton"];
            var startDateNowEdmonton = SystemClock.Instance.GetCurrentInstant().InZone(edmontonTz);
            var endDateNowEdmonton = SystemClock.Instance.GetCurrentInstant().InZone(edmontonTz).PlusHours(24 * 5);
            await Db.Location.AddAsync(new Location { Id = 1, AgencyId = "zz" });
            await Db.SaveChangesAsync();

            //await ShiftController.GetAvailability(startDateNowEdmonton, endDateNowEdmonton, )
        }


        private async Task<ShiftDto> CreateShift()
        {
            var sheriffId = Guid.NewGuid();
            await Db.Location.AddAsync(new Location { Id = 1, AgencyId = "zz", Timezone = "America/Vancouver"});
            await Db.Sheriff.AddAsync(new Sheriff { Id = sheriffId, FirstName = "First", LastName = "Sheriff"});
            await Db.SaveChangesAsync();

            var shiftDto = new ShiftDto
            {
                ExpiryDate = DateTimeOffset.UtcNow, // should be null.
                SheriffId = sheriffId, // should be null.
                Type = ShiftType.Jail,
                StartDate = DateTimeOffset.UtcNow,
                EndDate = DateTimeOffset.UtcNow.AddHours(5),
                Sheriff = new SheriffDto(),
                AnticipatedAssignment = null,
                Location = new LocationDto { Id = 55, AgencyId = "55" },
                LocationId = 1,
                Duties = new List<DutyDto>(),
            };
            return shiftDto;
        }

    }
}
