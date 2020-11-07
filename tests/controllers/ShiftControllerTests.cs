using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using SS.Api.controllers.scheduling;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.exceptions;
using SS.Api.Models.DB;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Api.services.usermanagement;
using SS.Common.helpers.extensions;
using ss.db.models;
using SS.Db.models.scheduling;
using SS.Db.models.scheduling.notmapped;
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
            ShiftController = new ShiftController(new ShiftService(Db, new SheriffService(Db)))
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task AddShiftConflicts()
        {
            var sheriffId = Guid.NewGuid();
            await Db.Location.AddAsync(new Location { Id = 1, AgencyId = "zz" });
            await Db.Sheriff.AddAsync(new Sheriff { Id = sheriffId, IsEnabled = true, HomeLocationId = 1});
            await Db.SaveChangesAsync();

            Detach();

            //Case where shifts conflict with themselves.
            var shiftOne = new Shift
            {
                StartDate = DateTimeOffset.UtcNow.Date,
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(1),
                LocationId = 1,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftTwo = new Shift
            {
                StartDate = DateTimeOffset.UtcNow.Date.AddHours(1),
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(2),
                LocationId = 1,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftThree = new Shift
            {
                Id = 3,
                StartDate = DateTimeOffset.UtcNow.Date,
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(1),
                LocationId = 1,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftFour = new Shift
            {
                Id = 4,
                StartDate = DateTimeOffset.UtcNow.Date.AddHours(1),
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(2),
                LocationId = 1,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftFive = new Shift
            {
                StartDate = DateTimeOffset.UtcNow.Date,
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(2),
                LocationId = 1,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftSix = new Shift
            {
                Id = 6,
                StartDate = DateTimeOffset.UtcNow.Date.AddHours(2),
                EndDate = DateTimeOffset.UtcNow.Date.AddHours(3),
                LocationId = 1,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();

            var shiftSeven = new Shift
            {
                Id = 7,
                StartDate = DateTimeOffset.UtcNow.Date.AddHours(-1),
                EndDate = DateTimeOffset.UtcNow.Date,
                LocationId = 1,
                Timezone = "America/Vancouver",
                SheriffId = sheriffId
            }.Adapt<AddShiftDto>();


            await Assert.ThrowsAsync<BusinessLayerException>(async () => await ShiftController.AddShifts( new List<AddShiftDto> { shiftOne, shiftFive } ));
            var sheriffShifts = Db.Shift.AsNoTracking().Where(s => s.SheriffId == sheriffId);
            Assert.Empty(sheriffShifts);

            //Two shifts no conflicts.
            var shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(new List<AddShiftDto> { shiftOne, shiftTwo }));
            sheriffShifts = Db.Shift.AsNoTracking().Where(s => s.SheriffId == sheriffId);
            Assert.All(sheriffShifts, s => new List<int> { 1, 2 }.Contains(s.Id));

            //Already assigned to two shifts: Two new shifts, should conflict now. 
            await Assert.ThrowsAsync<BusinessLayerException>(async () => await ShiftController.AddShifts(new List<AddShiftDto> { shiftThree, shiftFour }));
            sheriffShifts = Db.Shift.AsNoTracking().Where(s => s.SheriffId == sheriffId);
            Assert.All(sheriffShifts, s => new List<int> { 1, 2 }.Contains(s.Id));

            //Schedule two more shifts, on the outside of 3 and 4. 
            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(new List<AddShiftDto> { shiftSix, shiftSeven }));
            sheriffShifts = Db.Shift.AsNoTracking().Where(s => s.SheriffId == sheriffId);
            Assert.All(sheriffShifts, s => new List<int> { 3, 4, 6, 7 }.Contains(s.Id));

        }


        [Fact]
        public async Task GetAvailability()
        {
            await Db.Location.AddAsync(new Location { Id = 1, AgencyId = "zz", Name = "Location 1" });
            await Db.Location.AddAsync(new Location { Id = 2, AgencyId = "5555", Name = "Location 2" });
            await Db.SaveChangesAsync();

            var startDate = DateTimeOffset.UtcNow.ConvertToTimezone("America/Edmonton");
            var endDate = DateTimeOffset.UtcNow.TranslateDateIfDaylightSavings("America/Edmonton", 7);

            //On awayLocation.
            var awayLocationSheriff = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff
            {
                HomeLocationId = 1,
                Id = awayLocationSheriff,
                IsEnabled = true,
                AwayLocation = new List<SheriffAwayLocation>
                {
                    new SheriffAwayLocation
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(1),
                        LocationId = 2
                    }
                }
            });

            //On training.
            var trainingSheriff = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff
            {
                HomeLocationId = 1,
                Id = trainingSheriff,
                IsEnabled = true,
                Training = new List<SheriffTraining>
                {
                    new SheriffTraining
                    {
                        StartDate = startDate.AddDays(1),
                        EndDate = startDate.AddDays(2)
                    }
                }
            });

            //On leave.
            var leaveSheriff = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff
            {
                HomeLocationId = 1,
                Id = leaveSheriff,
                IsEnabled = true,
                Leave = new List<SheriffLeave>
                {
                    new SheriffLeave
                    {
                        StartDate = startDate.AddDays(1),
                        EndDate = startDate.AddDays(2)
                    }
                }
            });

            //Already scheduled.
            var scheduledSheriff = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff
            {
                HomeLocationId = 1,
                Id = scheduledSheriff,
                IsEnabled = true
            });

            await Db.Shift.AddAsync(new Shift
            {
                Id = 1,
                StartDate = startDate.AddDays(2),
                EndDate = startDate.AddDays(3),
                LocationId = 1,
                SheriffId = scheduledSheriff,
                Timezone = "America/Vancouver"
            });


            //Already scheduled different location.
            var scheduledDifferentLocationSheriff = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff
            {
                HomeLocationId = 1,
                Id = scheduledDifferentLocationSheriff,
                IsEnabled = true
            });

            await Db.Shift.AddAsync(new Shift
            {
                Id = 2,
                StartDate = startDate.AddDays(2),
                EndDate = startDate.AddDays(3),
                LocationId = 2,
                SheriffId = scheduledDifferentLocationSheriff,
                Timezone = "America/Vancouver"
            });

            //Expired Leave, Expired Training, Expired Away Location, Expired Shift.
            var expiredEventsAndShiftSheriff = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff
            {
                HomeLocationId = 1,
                Id = expiredEventsAndShiftSheriff,
                IsEnabled = true,
                Leave = new List<SheriffLeave>
                {
                    new SheriffLeave
                    {
                        StartDate = startDate.AddDays(1),
                        EndDate = startDate.AddDays(2),
                        ExpiryDate = DateTimeOffset.UtcNow
                    }
                },
                Training = new List<SheriffTraining>
                {
                    new SheriffTraining
                    {
                        StartDate = startDate.AddDays(1),
                        EndDate = startDate.AddDays(2),
                        ExpiryDate = DateTimeOffset.UtcNow
                    }
                },
                AwayLocation = new List<SheriffAwayLocation>
                {
                    new SheriffAwayLocation
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(1),
                        LocationId = 2,
                        ExpiryDate = DateTimeOffset.UtcNow
                    }
                }
            });

            await Db.Shift.AddAsync(new Shift
            {
                Id = 3,
                StartDate = startDate.AddDays(2),
                EndDate = startDate.AddDays(3),
                LocationId = 2,
                SheriffId = expiredEventsAndShiftSheriff,
                Timezone = "America/Vancouver",
                ExpiryDate = DateTimeOffset.UtcNow
            });

            //Expired Sheriff.
            var expiredSheriff = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff
            {
                HomeLocationId = 1,
                Id = expiredSheriff,
                FirstName = "Expired",
                LastName = "Expired Sheriff",
                IsEnabled = false
            });

            await Db.SaveChangesAsync();

            //Loaned in.
            var loanedInSheriff = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff
            {
                HomeLocationId = 2,
                FirstName = "Loaned In",
                LastName = "Loaned In",
                Id = loanedInSheriff,
                IsEnabled = true,
                AwayLocation = new List<SheriffAwayLocation>
                {
                    new SheriffAwayLocation
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(1),
                        LocationId = 1
                    }
                }
            });

            await Db.SaveChangesAsync();

            var shiftConflicts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await ShiftController.GetAvailability(1, startDate, endDate));

            //Postgres stores ticks as 1/1,000,000 vs .NET uses 1/10,000,000
            var awayLocationsSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == awayLocationSheriff);
            Assert.NotNull(awayLocationsSheriffConflicts); 
            Assert.Contains(awayLocationsSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.AwayLocation && c.LocationId == 2 && (startDate - c.Start).TotalSeconds <= 1 && (startDate.AddDays(1) - c.End).TotalSeconds <= 1);

            var trainingSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == trainingSheriff);
            Assert.NotNull(trainingSheriffConflicts);
            Assert.Contains(trainingSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.Training);

            var leaveSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == leaveSheriff);
            Assert.NotNull(leaveSheriffConflicts);
            Assert.Contains(leaveSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.Leave);

            var scheduledSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == scheduledSheriff);
            Assert.NotNull(scheduledSheriffConflicts);
            Assert.Contains(scheduledSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.Scheduled && (startDate.AddDays(2) - c.Start).TotalSeconds <= 1 && (startDate.AddDays(3) - c.End).TotalSeconds <= 1  && c.LocationId == 1);

            var scheduledDifferentLocationSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == scheduledDifferentLocationSheriff);
            Assert.NotNull(scheduledDifferentLocationSheriffConflicts);
            Assert.Contains(scheduledDifferentLocationSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.Scheduled && (startDate.AddDays(2) - c.Start).TotalSeconds <= 1 && (startDate.AddDays(3) - c.End).TotalSeconds <= 1 && c.LocationId == 2);

            var expiredEventsAndShiftSheriffConflict = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == expiredEventsAndShiftSheriff);
            Assert.NotNull(expiredEventsAndShiftSheriffConflict);
            Assert.Empty(expiredEventsAndShiftSheriffConflict.Conflicts);

            Assert.True(shiftConflicts.All(sc => sc.SheriffId != expiredSheriff));

            var loanedInSheriffConflicts = shiftConflicts.FirstOrDefault(sc => sc.SheriffId == loanedInSheriff);
            Assert.NotNull(loanedInSheriffConflicts);
            Assert.Contains(loanedInSheriffConflicts.Conflicts, c =>
                c.Conflict == ShiftConflictType.AwayLocation && c.LocationId == 1 && (startDate - c.Start).TotalSeconds <= 1 && (startDate.AddDays(1) - c.End).TotalSeconds <= 1);
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
                StartDate = startTimeOffset,
                EndDate = endTimeOffset,
                Sheriff = new Sheriff { Id = Guid.NewGuid(), LastName = "hello" },
                AnticipatedAssignment = new Assignment {Id = 1, Name = "Super assignment", Location = new Location { Id = 1, AgencyId = "zz"}, LookupCode = new LookupCode() {Id = 900000}},
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
            var shiftDtos = new List<AddShiftDto> {shiftDto.Adapt<AddShiftDto>()};
            var shift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos));
        }


        [Fact]
        public async Task AddShiftSheriffEventConflict()
        {
            await Db.Location.AddAsync(new Location { Id = 2, AgencyId = "5555", Name = "Location 2" });
            var shiftDto = await CreateShift();
        

            var startDate = shiftDto.StartDate;
            var sheriffId = Guid.NewGuid();
            shiftDto.SheriffId = sheriffId;

            var shiftDtos = new List<ShiftDto> { shiftDto }.Adapt<List<AddShiftDto>>();

            await Db.Sheriff.AddAsync(new Sheriff
            {
                Id = sheriffId, HomeLocationId = 1, FirstName = "First", LastName = "Sheriff", IsEnabled = true,
                Leave = new List<SheriffLeave>
                {
                    new SheriffLeave
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(2)
                    }
                },
                Training = new List<SheriffTraining>
                {
                    new SheriffTraining
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(2)
                    }
                },
                AwayLocation = new List<SheriffAwayLocation>
                {
                    new SheriffAwayLocation
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddDays(1),
                        LocationId = 2
                    }
                }
            });
            await Db.SaveChangesAsync();

            //Three conflicts should come back. 
            await Assert.ThrowsAsync<BusinessLayerException>(async () => await ShiftController.AddShifts(shiftDtos));
        }


        [Fact]
        public async Task UpdateShift()
        {
            var shiftDto = await CreateShift();
            var shiftDtos = new List<ShiftDto> {shiftDto};
            var sheriffId = Guid.NewGuid();
            await Db.Sheriff.AddAsync(new Sheriff { Id = sheriffId, FirstName = "Hello", LastName = "There", IsEnabled = true, HomeLocationId = 1});
            await Db.Assignment.AddAsync(new Assignment { Id = 5, LocationId = 1, LookupCode = new LookupCode() { Id = 9000 }});
            await Db.SaveChangesAsync();

            var shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos.Adapt<List<AddShiftDto>>()));
            var shift = shifts.First();
            shift.StartDate = DateTimeOffset.UtcNow.AddDays(5).Date;
            shift.EndDate = DateTimeOffset.UtcNow.AddDays(6).Date;
            shift.LocationId = 1; // This shouldn't change 
            shift.ExpiryDate = DateTimeOffset.UtcNow; // this shouldn't change
            shift.SheriffId = sheriffId;
            shift.Sheriff = new SheriffDto(); // shouldn't change
            shift.Duties = new List<DutyDto>(); // shouldn't change
            shift.AnticipatedAssignment = new AssignmentDto(); //this shouldn't create new. 
            shift.AnticipatedAssignmentId = 5;
            shift.Location = new LocationDto(); // shouldn't change

            var updatedShifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.UpdateShifts(shifts.Adapt<List<UpdateShiftDto>>()));
            var updatedShift = updatedShifts.First();

            Assert.Equal(shiftDto.LocationId,updatedShift.LocationId);
            Assert.Null(updatedShift.ExpiryDate);
            Assert.Equal(5, updatedShift.AnticipatedAssignmentId);
            Assert.Equal(sheriffId, updatedShift.SheriffId);

            //Create the same shift, without sheriff, should conflict.
            shiftDto.SheriffId = null;
            shiftDto.StartDate = DateTimeOffset.UtcNow.AddDays(5).Date;
            shiftDto.EndDate = DateTimeOffset.UtcNow.AddDays(6).Date;
            shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos.Adapt<List<AddShiftDto>>()));
            shift = shifts.First();

            shift.SheriffId = sheriffId;
            await Assert.ThrowsAsync<BusinessLayerException>(() => ShiftController.UpdateShifts(shifts.Adapt<List<UpdateShiftDto>>()));

            //Create a shift that sits side by side, without sheriff, shouldn't conflict.
            shiftDto.SheriffId = null;
            shiftDto.StartDate = DateTimeOffset.UtcNow.AddDays(4).Date;
            shiftDto.EndDate = DateTimeOffset.UtcNow.AddDays(5).Date;
            shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos.Adapt<List<AddShiftDto>>()));
            shift = shifts.First();

            shift.SheriffId = sheriffId;
            updatedShifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.UpdateShifts(shifts.Adapt<List<UpdateShiftDto>>()));
            updatedShift = updatedShifts.First();

            Assert.Equal(shiftDto.StartDate, updatedShift.StartDate);
            Assert.Equal(shiftDto.EndDate, updatedShift.EndDate);

            /*s.StartDate >= dateMin &&
            s.EndDate <= dateMax &&*/

            var stDate = DateTimeOffset.UtcNow.AddDays(20);
            var addShifts = new List<AddShiftDto>
            {
                new AddShiftDto
                {
                    SheriffId = sheriffId,
                    StartDate = stDate,
                    EndDate = stDate.AddHours(5),
                    LocationId = 1,
                    Timezone = "America/Edmonton"
                }
            };

            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(addShifts));

            addShifts = new List<AddShiftDto>
            {
                new AddShiftDto
                {
                    SheriffId = sheriffId,
                    StartDate = stDate.AddHours(2),
                    EndDate = stDate.AddHours(3),
                    LocationId = 1,
                    Timezone = "America/Edmonton"
                }
            };

            await Assert.ThrowsAsync<BusinessLayerException>(() => ShiftController.AddShifts(addShifts));

        }



        [Fact]
        public async Task RemoveShift()
        {
            var shiftDto = await CreateShift();
            var shiftDtos = new List<ShiftDto> {shiftDto}.Adapt<List<AddShiftDto>>();
            var shifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos));
            var shift = shifts.First();

            HttpResponseTest.CheckForNoContentResponse(await ShiftController.ExpireShift(shift.Id));

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.GetShifts(1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(5)));
            Assert.Empty(response);
        }

        [Fact]
        public async Task ImportWeeklyShifts()
        {
            var sheriffId = Guid.NewGuid();
            var shiftDto = await CreateShift();
            await Db.Sheriff.AddAsync(new Sheriff {Id = sheriffId, IsEnabled = true, HomeLocationId = 1});
            await Db.SaveChangesAsync();
            shiftDto.LocationId = 1;
            shiftDto.StartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6); //Last week monday
            shiftDto.EndDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 5); //Last week tuesday
            shiftDto.SheriffId = sheriffId;

            var shiftDtos = new List<ShiftDto> {shiftDto};
            var shift = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await ShiftController.AddShifts(shiftDtos.Adapt<List<AddShiftDto>>()));

            var importedShifts = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(
                await ShiftController.ImportWeeklyShifts(1, true, shiftDto.StartDate));

            Assert.NotNull(importedShifts);
            var importedShift = importedShifts.First();
            Assert.Equal(shiftDto.StartDate.AddDays(7).DateTime,importedShift.StartDate.DateTime, TimeSpan.FromSeconds(10)); //This week monday
            Assert.Equal(shiftDto.EndDate.AddDays(7).DateTime, importedShift.EndDate.DateTime, TimeSpan.FromSeconds(10)); //This week monday
            Assert.Equal(shiftDto.SheriffId, importedShift.SheriffId);
        }


        private async Task<ShiftDto> CreateShift()
        {
            var sheriffId = Guid.NewGuid();
            await Db.Location.AddAsync(new Location { Id = 1, AgencyId = "zz", Timezone = "America/Vancouver"});
            await Db.Sheriff.AddAsync(new Sheriff { Id = sheriffId, HomeLocationId = 1, FirstName = "First", LastName = "Sheriff", IsEnabled = true });
            await Db.SaveChangesAsync();

            var shiftDto = new ShiftDto
            {
                ExpiryDate = DateTimeOffset.UtcNow, // should be null.
                SheriffId = sheriffId, // should be null.
                StartDate = DateTimeOffset.UtcNow,
                EndDate = DateTimeOffset.UtcNow.AddHours(5),
                Sheriff = new SheriffDto(),
                AnticipatedAssignment = null,
                Location = new LocationDto { Id = 55, AgencyId = "55" },
                LocationId = 1,
                Duties = new List<DutyDto>(),
                Timezone = "America/Edmonton"
            };
            return shiftDto;
        }

    }
}
