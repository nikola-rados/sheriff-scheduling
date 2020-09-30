using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SS.Api.controllers.usermanagement;
using SS.Api.Models.DB;
using SS.Api.Models.Dto;
using SS.Api.services;
using ss.db.models;
using SS.Db.models;
using SS.Db.models.sheriff;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    public class SheriffControllerTests : WrapInTransactionScope
    {
        #region Variables
        private readonly SheriffController _controller;

        #endregion Variables

        public SheriffControllerTests() : base(false)
        {
            _controller = new SheriffController(new SheriffService(_dbContext), new UserService(_dbContext))
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

        [Fact]
        public async Task CreateSheriff()
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
                HomeLocationId = 1
            };

         

            var sheriffDto = newSheriff.Adapt<SheriffDto>();

            Debug.Write(JsonConvert.SerializeObject(sheriffDto));

            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(await _controller.CreateSheriff(sheriffDto));
            var sheriffResponse = response.Adapt<Sheriff>();

            Assert.NotNull(await _dbContext.Sheriff.FindAsync(sheriffResponse.Id));
        }

        [Fact]
        public async Task FindSheriff()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var controllerResult = await _controller.FindSheriff(sheriffObject.Id);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            var sheriffResponse = response.Adapt<Sheriff>();

            //Compare Sheriff and Sheriff Object.
            Assert.Equal(sheriffObject.FirstName, sheriffResponse.FirstName);
            Assert.Equal(sheriffObject.LastName, sheriffResponse.LastName);
            Assert.Equal(sheriffObject.BadgeNumber, sheriffResponse.BadgeNumber);
            Assert.Equal(sheriffObject.Email, sheriffResponse.Email);
            Assert.Equal(sheriffObject.Gender, sheriffResponse.Gender);
        }

        [Fact]
        public async Task UpdateSheriff()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var newLocation = new Location { Name = "5", Id = 5 , AgencyId = "645646464646464"};
            await _dbContext.Location.AddAsync(newLocation);
            var newLocation2 = new Location { Name = "6", Id = 6, AgencyId = "6456456464" };
            await _dbContext.Location.AddAsync(newLocation2);
            await _dbContext.SaveChangesAsync();

            Detach();

            var updateSheriff = sheriffObject.Adapt<SheriffDto>();
            updateSheriff.FirstName = "Al";
            updateSheriff.LastName = "Hoyne";
            updateSheriff.BadgeNumber = "55";
            updateSheriff.Email = "hey@hey.com";
            updateSheriff.Gender = Gender.Other;

            //This object is only used for fetching.
            //updateSheriff.HomeLocation = new LocationDto { Name = "Als place2", Id = 5};
            updateSheriff.HomeLocationId = 5;

            Detach();

            var controllerResult = await _controller.UpdateSheriff(updateSheriff);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            var sheriffResponse = response.Adapt<Sheriff>();

            Assert.Equal(updateSheriff.FirstName, sheriffResponse.FirstName);
            Assert.Equal(updateSheriff.LastName, sheriffResponse.LastName);
            Assert.Equal(updateSheriff.BadgeNumber, sheriffResponse.BadgeNumber);
            Assert.Equal(updateSheriff.Email, sheriffResponse.Email);
            Assert.Equal(updateSheriff.Gender, sheriffResponse.Gender);

            //Shouldn't be able to update any of the navigation properties here.
            Assert.Empty(sheriffResponse.AwayLocation);
            Assert.Empty(sheriffResponse.Training);
            Assert.Empty(sheriffResponse.Leave);
            //Set to invalid location, should be null.
            //WE didn't set the HomeLocationId here. 
            Assert.NotNull(sheriffResponse.HomeLocation);

            Detach();

            updateSheriff.HomeLocationId = 6;
            updateSheriff.HomeLocation = newLocation2.Adapt<LocationDto>();
            controllerResult = await _controller.UpdateSheriff(updateSheriff);
            response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Detach();

            var controllerResult2 = await _controller.FindSheriff(sheriffResponse.Id);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);
            Assert.NotNull(response2.HomeLocation);
            Assert.Equal(6, response.HomeLocation.Id);
        }



        [Fact]
        public async void AddUpdateRemoveSheriffAwayLocation()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var newLocation = new Location { Name = "New PLace", AgencyId = "545325345353"};
            await _dbContext.Location.AddAsync(newLocation);
            var newLocation2 = new Location { Name = "New PLace", AgencyId = "g435346346363"};
            await _dbContext.Location.AddAsync(newLocation2);
            await _dbContext.SaveChangesAsync();

            var sheriffAwayLocation = new SheriffAwayLocationDto
            {
                IsFullDay = true,
                SheriffId =  sheriffObject.Id,
                LocationId = newLocation.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            //Add
            var controllerResult = await _controller.AddSheriffAwayLocation(sheriffAwayLocation);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(sheriffAwayLocation.LocationId, response.Location.Id);
            Assert.Equal(sheriffAwayLocation.IsFullDay, response.IsFullDay);
            Assert.Equal(sheriffAwayLocation.SheriffId, response.SheriffId);
            Assert.Equal(sheriffAwayLocation.StartDate, response.StartDate);
            Assert.Equal(sheriffAwayLocation.EndDate, response.EndDate);

            Detach();

            var controllerResult2 = await _controller.FindSheriff(sheriffObject.Id);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            Assert.True(response2.AwayLocation.Count == 1);

            var updateSheriffAwayLocation = sheriffAwayLocation.Adapt<SheriffAwayLocationDto>();
            updateSheriffAwayLocation.StartDate = DateTime.UtcNow.AddDays(5);
            updateSheriffAwayLocation.EndDate = DateTime.UtcNow.AddDays(5);
            updateSheriffAwayLocation.IsFullDay = false;
            updateSheriffAwayLocation.LocationId = newLocation2.Id;
            updateSheriffAwayLocation.Id = response.Id;

            Detach();

            //Update
            var controllerResult3 = await _controller.UpdateSheriffAwayLocation(updateSheriffAwayLocation);
            var response3 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);

            Assert.Equal(response3.StartDate, updateSheriffAwayLocation.StartDate);
            Assert.Equal(response3.EndDate, updateSheriffAwayLocation.EndDate);
            Assert.Equal(response3.IsFullDay, updateSheriffAwayLocation.IsFullDay);
            Assert.Equal(response3.SheriffId, updateSheriffAwayLocation.SheriffId);

            Detach();

            //Remove
            var controllerResult4 = await _controller.RemoveSheriffAwayLocation(response.Id);
            HttpResponseTest.CheckForNoContentResponse(controllerResult4);

            var controllerResult6 = await _controller.FindSheriff(sheriffObject.Id);
            var response6 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult6);
            Assert.Empty(response6.AwayLocation);
        }

        [Fact]
        public async void AddUpdateRemoveSheriffLeave()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();
            var newLocation = new Location { Name = "New PLace", AgencyId = "gfgdf43535345"};
            await _dbContext.Location.AddAsync(newLocation);

            await _dbContext.SaveChangesAsync();

            var lookupCode = new LookupCode
            {
                Code = "zz",
                Description = "gg",
                LocationId = newLocation.Id
            };
            await _dbContext.LookupCode.AddAsync(lookupCode);

            var lookupCode2 = new LookupCode
            {
                Code = "zz",
                Description = "gg",
                LocationId = newLocation.Id
            };

            await _dbContext.LookupCode.AddAsync(lookupCode2);

            await _dbContext.SaveChangesAsync();


            var entity = new SheriffLeaveDto
            {
                LeaveTypeId = lookupCode.Id,
                IsFullDay = true,
                SheriffId = sheriffObject.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            //Add
            var controllerResult = await _controller.AddSheriffLeave(entity);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Assert.Equal(entity.LeaveTypeId, response.LeaveType.Id);
            Assert.Equal(entity.IsFullDay, response.IsFullDay);
            Assert.Equal(entity.SheriffId, response.SheriffId);
            Assert.Equal(entity.StartDate, response.StartDate);
            Assert.Equal(entity.EndDate, response.EndDate);

            var controllerResult2 = await _controller.FindSheriff(sheriffObject.Id);
            var response2 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult2);

            Assert.True(response2.Leave.Count == 1);

            Detach();

            var updateSheriffLeave = entity.Adapt<SheriffLeaveDto>();
            updateSheriffLeave.StartDate = DateTime.UtcNow.AddDays(5);
            updateSheriffLeave.EndDate = DateTime.UtcNow.AddDays(5);
            updateSheriffLeave.IsFullDay = false;
            //updateSheriffLeave.LeaveTypeId = lookupCode2.Id;
           
            updateSheriffLeave.LeaveType = lookupCode2.Adapt<LookupCodeDto>();
            updateSheriffLeave.LeaveTypeId = lookupCode2.Id;
            updateSheriffLeave.Id = response.Id;

            //Update
            var controllerResult3 = await _controller.UpdateSheriffLeave(updateSheriffLeave);
            var response3 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);

            Assert.Equal(response3.StartDate, updateSheriffLeave.StartDate);
            Assert.Equal(response3.EndDate, updateSheriffLeave.EndDate);
            Assert.Equal(response3.IsFullDay, updateSheriffLeave.IsFullDay);
            Assert.Equal(response3.SheriffId, updateSheriffLeave.SheriffId);
            Assert.Equal(response3.LeaveType.Id, updateSheriffLeave.LeaveTypeId);

            //Remove
            var controllerResult4 = await _controller.RemoveSheriffLeave(updateSheriffLeave.Id);
            HttpResponseTest.CheckForNoContentResponse(controllerResult4);

            var controllerResult5 = await _controller.FindSheriff(sheriffObject.Id);
            var response5 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult5);
            Assert.Empty(response5.Leave);
        }
       
        [Fact]
        public async void AddUpdateRemoveSheriffTraining()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var newLocation = new Location { Name = "New PLace", AgencyId = "zfddf2342"};
            await _dbContext.Location.AddAsync(newLocation);

            await _dbContext.SaveChangesAsync();

            var lookupCode = new LookupCode
            {
                Code = "zz",
                Description = "gg",
                LocationId = newLocation.Id
            };
            await _dbContext.LookupCode.AddAsync(lookupCode);

            var lookupCode2 = new LookupCode
            {
                Code = "zz",
                Description = "gg",
                LocationId = newLocation.Id
            };

            await _dbContext.LookupCode.AddAsync(lookupCode2);

            await _dbContext.SaveChangesAsync();


            var entity = new SheriffTrainingDto
            {
                TrainingTypeId = lookupCode.Id,
                IsFullDay = true,
                SheriffId = sheriffObject.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            //Add
            var controllerResult = await _controller.AddSheriffTraining(entity);
            var response = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult);

            Detach();

            var updateSheriffTraining = entity.Adapt<SheriffTrainingDto>();
            updateSheriffTraining.StartDate = DateTime.UtcNow.AddDays(5);
            updateSheriffTraining.EndDate = DateTime.UtcNow.AddDays(5);
            updateSheriffTraining.IsFullDay = false;
            updateSheriffTraining.TrainingTypeId = lookupCode2.Id;
            updateSheriffTraining.TrainingType = lookupCode2.Adapt<LookupCodeDto>();
            updateSheriffTraining.Id = response.Id;

            //Update
            var controllerResult3 = await _controller.UpdateSheriffTraining(updateSheriffTraining);
            var response3 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult3);

            Assert.Equal(response3.StartDate, updateSheriffTraining.StartDate);
            Assert.Equal(response3.EndDate, updateSheriffTraining.EndDate);
            Assert.Equal(response3.IsFullDay, updateSheriffTraining.IsFullDay);
            Assert.Equal(response3.SheriffId, updateSheriffTraining.SheriffId);
            Assert.Equal(response3.TrainingType.Id, updateSheriffTraining.TrainingTypeId);

            //Remove
            var controllerResult4 = await _controller.RemoveSheriffTraining(updateSheriffTraining.Id);
            HttpResponseTest.CheckForNoContentResponse(controllerResult4);

            var controllerResult5 = await _controller.FindSheriff(sheriffObject.Id);
            var response5 = HttpResponseTest.CheckForValid200HttpResponseAndReturnValue(controllerResult5);
            Assert.Empty(response5.Training);
        }

        #region Helpers


        private async Task<Sheriff> CreateSheriffUsingDbContext()
        {
            var newSheriff = new Sheriff
            {
                FirstName = "Ted",
                LastName = "Tums",
                BadgeNumber = "555",
                Email = "Ted@Teddy.com",
                Gender = Gender.Female,
                Id = new Guid(),
                IdirId = new Guid(),
                IdirName = "ted@fakeidir",
                HomeLocation =  new Location { Name = "Teds Place", AgencyId = "5555555435353535353535353"},
            };

            await _dbContext.Sheriff.AddAsync(newSheriff);
            await _dbContext.SaveChangesAsync();

            Detach();
            
            return newSheriff;
        }


        private async Task<SheriffAwayLocation> CreateSheriffAwayLocationUsingDbContext()
        {
            var sheriffObject = await CreateSheriffUsingDbContext();

            var newLocation = new Location { Name = "New PLace" };
            await _dbContext.Location.AddAsync(newLocation);
            await _dbContext.SaveChangesAsync();

            Detach();

            var sheriffAwayLocation = new SheriffAwayLocation
            {
                LocationId = newLocation.Id,
                IsFullDay = true,
                SheriffId = sheriffObject.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(3)
            };

            await _dbContext.SheriffAwayLocation.AddAsync(sheriffAwayLocation);
            await _dbContext.SaveChangesAsync();

            Detach();

            return sheriffAwayLocation;
        }



        #endregion Helpers
    }
}
