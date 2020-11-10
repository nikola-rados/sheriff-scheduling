using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mapster;
using SS.Api.controllers.scheduling;
using SS.Api.infrastructure.exceptions;
using SS.Api.Models.DB;
using SS.Api.models.dto.generated;
using SS.Api.services.scheduling;
using SS.Db.models.scheduling;
using SS.Db.models.sheriff;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    //Sequential, because there are issues with Adding Location (with a unique index) within a TransactionScope.
    [Collection("Sequential")]
    public class DutyRosterControllerTests : WrapInTransactionScope
    {
        #region Fields
        private readonly DutyRosterController _controller;
        #endregion Fields

        public DutyRosterControllerTests() : base(false)
        {
            _controller = new DutyRosterController(new DutyRosterService(Db))
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task GetDuties()
        {
            var location = new Location {Id = 1, AgencyId = "5555", Name = "dfd"};
            await Db.Location.AddAsync(location);

            var startDate = DateTimeOffset.UtcNow;
            var addDuty = new Duty
            {
                Id = 1,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                LocationId = location.Id
            };

            var addDutyExpiryDate = new Duty
            {
                Id = 2,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                ExpiryDate = startDate,
                LocationId = location.Id
            };

            await Db.Duty.AddAsync(addDuty);
            await Db.Duty.AddAsync(addDutyExpiryDate);
            await Db.SaveChangesAsync();

            var controllerResult = await _controller.GetDuties(1, startDate, startDate.AddDays(1));
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.Single(response);
            Assert.Equal(1, response.First().Id);
        }

        [Fact]
        public async Task AddDuty()
        {
            var location = new Location { Id = 1, AgencyId = "5555", Name = "dfd" };
            await Db.Location.AddAsync(location);
            await Db.SaveChangesAsync();

            var startDate = DateTimeOffset.UtcNow;
            var addDuty = new AddDutyDto
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                LocationId = 1,
                Timezone = "America/Vancouver"
            };
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddDuty(addDuty));
            Assert.NotNull(response);
            Assert.Equal(addDuty.StartDate, response.StartDate);
            Assert.Equal(addDuty.EndDate, response.EndDate);
        }

        [Fact]
        public async Task ExpireDuty()
        {
            var location = new Location { Id = 1, AgencyId = "5555", Name = "dfd" };
            await Db.Location.AddAsync(location);

            var startDate = DateTimeOffset.UtcNow.AddYears(5);
            var addDuty = new Duty
            {
                Id = 1,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                LocationId = 1
            };
            await Db.Duty.AddAsync(addDuty);
            await Db.SaveChangesAsync();

            HttpResponseTest.CheckForNoContentResponse(await _controller.ExpireDuty(1));

            var controllerResult = await _controller.GetDuties(1, startDate, startDate.AddDays(1));
            var getDuties = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.Empty(getDuties);
        }

        [Fact]
        public async Task DutySlotOverlapSelf()
        {
            var location = new Location { Id = 1, AgencyId = "5555", Name = "dfd" };
            await Db.Location.AddAsync(location);

            var newSheriff = new Sheriff
            {
                FirstName = "Ted",
                LastName = "Tums",
                HomeLocationId = location.Id,
                IsEnabled = true
            };

            await Db.Sheriff.AddAsync(newSheriff);
            await Db.SaveChangesAsync();

            var startDate = DateTimeOffset.UtcNow.AddYears(5);
            var addDuty = new AddDutyDto
            {
                LocationId = 1,
                StartDate = startDate, 
                EndDate = startDate.AddDays(5),
                Timezone = "America/Vancouver"
            };
            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.AddDuty(addDuty));

            var controllerResult = await _controller.GetDuties(location.Id, startDate, startDate.AddDays(5));
            var getDuties = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Single(getDuties);
            var duty = getDuties.First().Adapt<UpdateDutyDto>();

            duty.DutySlots = new List<UpdateDutySlotDto>()
            {
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    SheriffId = newSheriff.Id
                },
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    SheriffId = newSheriff.Id
                }
            };

            //Should throw self conflicting exception.
            await Assert.ThrowsAsync<BusinessLayerException>(async () => await  _controller.UpdateDuties(new List<UpdateDutyDto> { duty }));


            duty.DutySlots = new List<UpdateDutySlotDto>()
            {
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    SheriffId = newSheriff.Id
                },
                new UpdateDutySlotDto
                {
                    StartDate = startDate.AddDays(5),
                    EndDate = startDate.AddDays(6),
                    SheriffId = newSheriff.Id
                }
            };

            //Shouldn't overlap. 
            var controllerResult2 = await _controller.UpdateDuties(new List<UpdateDutyDto> { duty });
            var getDuties2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);
        }

        [Fact]
        public async Task DutySlotOverlap()
        {
            var location = new Location { Id = 1, AgencyId = "5555", Name = "dfd" };
            await Db.Location.AddAsync(location);

            var newSheriff = new Sheriff
            {
                FirstName = "Ted",
                LastName = "Tums",
                HomeLocationId = location.Id,
                IsEnabled = true
            };

            await Db.Sheriff.AddAsync(newSheriff);
            await Db.SaveChangesAsync();

            var startDate = DateTimeOffset.UtcNow.AddYears(5);

            var duty = new Duty
            {
                Id = 1,
                LocationId = 1,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                Timezone = "America/Vancouver"
            };

            var duty2 = new Duty
            {
                Id = 2,
                LocationId = 1,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                Timezone = "America/Vancouver"
            };

            await Db.Duty.AddAsync(duty);
            await Db.Duty.AddAsync(duty2);
            await Db.SaveChangesAsync();

            var controllerResult = await _controller.GetDuties(location.Id, startDate, startDate.AddDays(5));
            var getDuties = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.NotNull(getDuties);
            Assert.NotNull(getDuties.FirstOrDefault(d => d.Id == 1));
            Assert.NotNull(getDuties.FirstOrDefault(d => d.Id == 2));

            var updateDuty = getDuties.FirstOrDefault(d => d.Id == 1)!.Adapt<UpdateDutyDto>();
            updateDuty.DutySlots = new List<UpdateDutySlotDto>
            {
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    SheriffId = newSheriff.Id
                }
            };

            var controllerResult2 = await _controller.UpdateDuties(new List<UpdateDutyDto> {updateDuty} );
            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            var updateDuty2 = getDuties.FirstOrDefault(d => d.Id == 2)!.Adapt<UpdateDutyDto>();
            updateDuty2.DutySlots = new List<UpdateDutySlotDto>
            {
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    SheriffId = newSheriff.Id
                }
            };

            //Should conflict. 
            await Assert.ThrowsAsync<BusinessLayerException>(async () => await _controller.UpdateDuties(new List<UpdateDutyDto> { updateDuty2 }));
        }


        [Fact]
        public async Task AddUpdateRemoveDutySlots()
        {
            var location = new Location { Id = 1, AgencyId = "5555", Name = "dfd" };
            await Db.Location.AddAsync(location);

            var newSheriff = new Sheriff
            {
                FirstName = "Ted",
                LastName = "Tums",
                HomeLocationId = location.Id,
                IsEnabled = true
            };

            await Db.Sheriff.AddAsync(newSheriff);
            await Db.SaveChangesAsync();

            var startDate = DateTimeOffset.UtcNow.AddYears(5);

            var duty = new Duty
            {
                Id = 1,
                LocationId = 1,
                StartDate = startDate,
                EndDate = startDate.AddDays(5),
                Timezone = "America/Vancouver"
            };

            await Db.Duty.AddAsync(duty);
            await Db.SaveChangesAsync();

            var controllerResult = await _controller.GetDuties(location.Id, startDate, startDate.AddDays(5));
            var getDuties = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.NotEmpty(getDuties);

            
            var updateDuty = getDuties.FirstOrDefault(d => d.Id == 1)!.Adapt<UpdateDutyDto>();
            updateDuty.DutySlots = new List<UpdateDutySlotDto>
            {
                //Add.
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    SheriffId = newSheriff.Id
                },
                //Add.
                new UpdateDutySlotDto
                {
                    StartDate = startDate.AddDays(5),
                    EndDate = startDate.AddDays(10),
                    SheriffId = newSheriff.Id
                },
            };

            var controllerResult2 = await _controller.UpdateDuties(new List<UpdateDutyDto> { updateDuty });
            var updateDuty2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            updateDuty.DutySlots = new List<UpdateDutySlotDto>
            {
                //Update
                new UpdateDutySlotDto
                {
                    Id = updateDuty2.First().DutySlots.First().Id,
                    StartDate = startDate.AddDays(10),
                    EndDate = startDate.AddDays(15),
                    SheriffId = newSheriff.Id
                },
                //Add
                new UpdateDutySlotDto
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(5),
                    SheriffId = newSheriff.Id
                },
                //Implicit remove of one. 
            };

            var controllerResult3 = await _controller.UpdateDuties(new List<UpdateDutyDto> { updateDuty });
            HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);
        }
    }
}
