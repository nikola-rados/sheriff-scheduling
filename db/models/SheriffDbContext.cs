using System;
using db.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using SS.Api.models;
using ss.db.models;
using SS.Db.models.auth;

namespace SS.Api.Models.DB
{
    public partial class SheriffDbContext : DbContext
    {
        public SheriffDbContext()
        {

        }

        public SheriffDbContext(DbContextOptions<SheriffDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<LookupCode> LookupCode { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<User> AuthUser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Name=ConnectionStrings.DB");
            }
        }

    }
}
