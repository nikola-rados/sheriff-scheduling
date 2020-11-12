using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using db.models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SS.Api.Models.DB;
using ss.db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;
using Microsoft.AspNetCore.Http;
using SS.Common.authorization;
using SS.Db.models.lookupcodes;
using SS.Db.models.scheduling;

namespace SS.Db.models
{
    public partial class SheriffDbContext : DbContext, IDataProtectionKeyContext
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

        #region Scheduling
        public virtual DbSet<Shift> Shift { get; set; }
        public virtual DbSet<Assignment> Assignment { get; set; }
        public virtual DbSet<Duty> Duty { get; set; }
        public virtual DbSet<DutySlot> DutySlot { get; set; }
        #endregion Scheduling

        // This maps to the table that stores keys.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

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

        //Only used in tests. 
        public override int SaveChanges()
        {
            HandleSaveChanges();
            return base.SaveChanges();
        }
        
        /// <summary>
        /// Save the entities with who created them or updated them.
        /// </summary>
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
        public TEntity DetachedClone<TEntity>(TEntity entity) where TEntity : class
            => Entry(entity).CurrentValues.Clone().ToObject() as TEntity;

        private Guid? GetUserId(string claimValue)
        {
            if (claimValue == null)
                return null;
            return Guid.Parse(claimValue);
        }
    }
}
