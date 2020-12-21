using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using db.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SS.Api.Controllers;
using SS.Api.infrastructure.authorization;
using SS.Api.Models.DB;
using SS.Common.authorization;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;
using tests.api.helpers;
using Xunit;
using static System.String;

namespace tests
{
    //Test out some of the claim based permissions on our controllers. 
    public class PermissionTests
    {
        private Guid _ownUserId;

        public PermissionTests()
        {
            _ownUserId = Guid.NewGuid();
        }

        [Fact]
        public Task ListControllersAndMethodsWithNoPermissions()
        {
            var results = Assembly.GetAssembly(typeof(AuthController))
                ?.GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                .SelectMany(type =>
                    type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                        true)
                    .Any())
                .Select(x => new
                {
                    Controller = x.DeclaringType?.Name,
                    Action = x.Name,
                    ReturnType = x.ReturnType.Name,
                    Attributes = Join(",",
                        x.GetCustomAttributes()
                            .Select(a => a.GetType()
                                .Name.Replace("Attribute",
                                    ""))),
                    ControllerAttribute = Join(",",
                        x.DeclaringType!.GetCustomAttributes()
                            .Select(a => a.GetType()
                                .Name.Replace("Attribute",
                                    "")))
                })
                .Where(m => !m.Attributes.Contains("Authorize") && !m.ControllerAttribute.Contains("Authorize"))
                .OrderBy(x => x.Controller)
                .ThenBy(x => x.Action)
                .ToList();

            Assert.True(results != null && results.Count == 2);
            Assert.Contains(results, results => results.Action.Contains("Logout"));
            Assert.Contains(results, results => results.Action.Contains("GetToken"));

            return Task.CompletedTask;
        }

        [Fact]
        public async Task SheriffDataTask()
        {
            //Runs at the highest tier.
            //1. ViewProfilesInAllLocation only.
            //2. ViewProfilesInOwnLocation only.
            //3. ViewOwnProfile only.
            //4. None

            var options = new DbContextOptionsBuilder<SheriffDbContext>()
                .UseInMemoryDatabase("SheriffTestDb")
                .Options;

            await using var db = new MemorySheriffDbContext(options);

            await db.Sheriff.AddAsync(new Sheriff { Id = new Guid(), HomeLocation = new Location { Id = 1 } });
            await db.Sheriff.AddAsync(new Sheriff { Id = new Guid(), HomeLocation = new Location { Id = 2 } });
            //Loaned in case.
            await db.Sheriff.AddAsync(new Sheriff
            {
                Id = new Guid(),
                HomeLocation = new Location
                {
                    Id = 3
                },
                AwayLocation = new List<SheriffAwayLocation>
                {
                    new SheriffAwayLocation
                    {
                        Id = 1,
                        LocationId = 4,
                        StartDate = DateTimeOffset.UtcNow.AddDays(4),
                        EndDate = DateTimeOffset.UtcNow.AddDays(20),
                        ExpiryDate = null,
                        SheriffId = _ownUserId
                    }
                }
            });
        
            await db.Sheriff.AddAsync(new Sheriff { Id = _ownUserId, HomeLocation = new Location { Id = 4 }});
            //Future loan case, shouldn't show up. 
            await db.Sheriff.AddAsync(new Sheriff
            {
                Id = new Guid(),
                HomeLocation = new Location
                {
                    Id = 5
                },
                AwayLocation = new List<SheriffAwayLocation>
                {
                    new SheriffAwayLocation
                    {
                        Id = 2,
                        LocationId = 4,
                        StartDate = DateTimeOffset.UtcNow.AddDays(8),
                        EndDate = DateTimeOffset.UtcNow.AddDays(20),
                        ExpiryDate = null,
                        SheriffId = _ownUserId
                    }
                }
            });
            await db.SaveChangesAsync();

            var user = SetupClaimsPrincipal(new List<Claim>
            {
                new Claim(CustomClaimTypes.HomeLocationId, "4"),
                new Claim(CustomClaimTypes.Permission, Permission.ViewProvince)
            });

            var start = DateTimeOffset.UtcNow.Date;
            var end = start.AddDays(7);

            var sheriffs = await db.Sheriff.AsNoTracking().ApplyPermissionFilters(user, start, end, db).ToListAsync();

            Assert.True(sheriffs.Count == 5);

            user = SetupClaimsPrincipal(new List<Claim>
            {
                new Claim(CustomClaimTypes.HomeLocationId, "4"),
                new Claim(CustomClaimTypes.Permission, Permission.ViewHomeLocation),
            });

            sheriffs = await db.Sheriff.AsNoTracking().ApplyPermissionFilters(user, start, end, db).ToListAsync();

            Assert.True(sheriffs.Count == 2);

            user = SetupClaimsPrincipal(new List<Claim>
            {
                new Claim(CustomClaimTypes.HomeLocationId, "4"),
                new Claim(CustomClaimTypes.Permission, Permission.ViewHomeLocation),
            });

            sheriffs = await db.Sheriff.AsNoTracking().ApplyPermissionFilters(user, start, end, db).ToListAsync();

            Assert.True(sheriffs.Count == 1);

            user = SetupClaim();

            sheriffs = await db.Sheriff.AsNoTracking().ApplyPermissionFilters(user, start, end, db).ToListAsync();

            Assert.Empty(sheriffs);
            await db.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task LocationDataTask()
        {
            var options = new DbContextOptionsBuilder<SheriffDbContext>()
                .UseInMemoryDatabase("SheriffTestDb")
                .Options;

            await using var db = new MemorySheriffDbContext(options);
            await db.Location.AddAsync(new Location { Id = 1 });
            await db.Location.AddAsync(new Location { Id = 2 });
            await db.Location.AddAsync(new Location { Id = 4, Region = new Region { Id = 1 } });
            await db.Location.AddAsync(new Location { Id = 3, RegionId = 1 });
            await db.Location.AddAsync(new Location { Id = 5 });
            await db.SheriffAwayLocation.AddAsync(new SheriffAwayLocation
            {
                Id = 1,
                LocationId = 5,
                StartDate = DateTimeOffset.UtcNow.AddDays(4),
                EndDate = DateTimeOffset.UtcNow.AddDays(20),
                SheriffId = _ownUserId
            });
            await db.SaveChangesAsync();

            //Cascading filter. 
            //1. Permission.ViewProvince only.
            //2. Permission.ViewRegion || Permission.ViewAssignedLocation || Permission.ViewHomeLocation
            //3. None

            var user = SetupClaim(CustomClaimTypes.Permission, Permission.ViewProvince);

            var locations = await db.Location.AsNoTracking().ApplyPermissionFilters(user, db).ToListAsync();

            Assert.True(locations.Count == 5);

            user = SetupClaimsPrincipal(new List<Claim>
            {
                new Claim(CustomClaimTypes.HomeLocationId, "4"),
                new Claim(CustomClaimTypes.Permission, Permission.ViewHomeLocation),
            });

            locations = await db.Location.AsNoTracking().ApplyPermissionFilters(user, db).ToListAsync();

            Assert.True(locations.Count == 1);

            user = SetupClaimsPrincipal(new List<Claim>
            {
                new Claim(CustomClaimTypes.HomeLocationId, "4"),
                new Claim(CustomClaimTypes.Permission, Permission.ViewAssignedLocation),
                new Claim(CustomClaimTypes.Permission, Permission.ViewHomeLocation),
            });

            locations = await db.Location.AsNoTracking().ApplyPermissionFilters(user, db).ToListAsync();
            
            Assert.True(locations.Count == 2);

            user = SetupClaimsPrincipal(new List<Claim>
            {
                new Claim(CustomClaimTypes.HomeLocationId, "4"),
                new Claim(CustomClaimTypes.Permission, Permission.ViewRegion),
                new Claim(CustomClaimTypes.Permission, Permission.ViewAssignedLocation),
                new Claim(CustomClaimTypes.Permission, Permission.ViewHomeLocation),
            });

            locations = await db.Location.AsNoTracking().ApplyPermissionFilters(user, db).ToListAsync();

            Assert.True(locations.Count == 3);

            user = SetupClaim();

            locations = await db.Location.AsNoTracking().ApplyPermissionFilters(user, db).ToListAsync();

            Assert.Empty(locations);
            await db.Database.EnsureDeletedAsync();
        }

        #region Helpers
        private ClaimsPrincipal SetupClaim()
        {
            return SetupClaimsPrincipal(new List<Claim>());
        }

        private ClaimsPrincipal SetupClaim(string type, string value)
        {
            return SetupClaimsPrincipal(new List<Claim>
            {
                new Claim(type, value)
            });
        }

        private ClaimsPrincipal SetupClaimsPrincipal(IEnumerable<Claim> claims)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(claims.Append(new Claim(CustomClaimTypes.UserId,
                    _ownUserId.ToString())),
                "mock"));
        }
        #endregion

    }
}
