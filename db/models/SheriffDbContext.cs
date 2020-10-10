using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using db.models;
using Microsoft.EntityFrameworkCore;
using SS.Api.Models.DB;
using SS.Db.configuration;
using SS.DB.Configuration;
using ss.db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;
using Microsoft.AspNetCore.Http;
using SS.Api.infrastructure.authorization;
using SS.Db.models.lookupcodes;

namespace SS.Db.models
{
    public partial class SheriffDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SheriffDbContext()
        {

        }

        public SheriffDbContext(DbContextOptions<SheriffDbContext> options, IHttpContextAccessor httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<LookupCode> LookupCode { get; set; }
        public virtual DbSet<LookupSortOrder> LookupSortOrder { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Sheriff> Sheriff { get; set; }
        public virtual DbSet<SheriffLeave> SheriffLeave { get; set; }
        public virtual DbSet<SheriffAwayLocation> SheriffAwayLocation { get; set; }
        public virtual DbSet<SheriffTraining> SheriffTraining { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Role> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyAllConfigurations();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Name=DatabaseConnectionString");
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }
        //Credit to PIMS for this. 
        /// <summary>
        /// Save the entities with who created them or updated them.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            HandleSaveChanges();
            return base.SaveChanges();
        }

        private void HandleSaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            var userId = GetUserId(_httpContextAccessor?.HttpContext?.User.FindFirst(CustomClaimTypes.UserId)?.Value);
            userId ??= auth.User.SystemUser;
            foreach (var entry in modifiedEntries)
            {
                if (entry.Entity is BaseEntity entity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedById = userId;
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    else if (entry.State != EntityState.Deleted)
                    {
                        entity.UpdatedById = userId;
                        entity.UpdatedOn = DateTime.UtcNow;
                    }
                }
            }
        }

        private Guid? GetUserId(string claimValue)
        {
            if (claimValue == null)
                return null;
            return Guid.Parse(claimValue);
        }
    }
}
