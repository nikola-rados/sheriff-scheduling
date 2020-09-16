using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<Assignment> Assignment { get; set; }
        public virtual DbSet<AudSheriffDuty> AudSheriffDuty { get; set; }
        public virtual DbSet<AuthRole> AuthRole { get; set; }
        public virtual DbSet<AuthRolePermission> AuthRolePermission { get; set; }
        public virtual DbSet<AuthUser> AuthUser { get; set; }
        public virtual DbSet<AuthUserRole> AuthUserRole { get; set; }
        public virtual DbSet<CourtRoleCode> CourtRoleCode { get; set; }
        public virtual DbSet<Courtroom> Courtroom { get; set; }
        public virtual DbSet<Databasechangelog> Databasechangelog { get; set; }
        public virtual DbSet<Databasechangeloglock> Databasechangeloglock { get; set; }
        public virtual DbSet<Duty> Duty { get; set; }
        public virtual DbSet<DutyRecurrence> DutyRecurrence { get; set; }
        public virtual DbSet<EscortRun> EscortRun { get; set; }
        public virtual DbSet<GenderCode> GenderCode { get; set; }
        public virtual DbSet<JailRoleCode> JailRoleCode { get; set; }
        public virtual DbSet<Leave> Leave { get; set; }
        public virtual DbSet<LeaveCancelReasonCode> LeaveCancelReasonCode { get; set; }
        public virtual DbSet<LeaveCode> LeaveCode { get; set; }
        public virtual DbSet<LeaveSubCode> LeaveSubCode { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<OtherAssignCode> OtherAssignCode { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Sheriff> Sheriff { get; set; }
        public virtual DbSet<SheriffDuty> SheriffDuty { get; set; }
        public virtual DbSet<SheriffLocation> SheriffLocation { get; set; }
        public virtual DbSet<SheriffRankCode> SheriffRankCode { get; set; }
        public virtual DbSet<Shift> Shift { get; set; }
        public virtual DbSet<WorkSectionCode> WorkSectionCode { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Name=ConnectionStrings.DB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.ToTable("assignment", "shersched");

                entity.HasIndex(e => e.CourtRoleId)
                    .HasName("ix_assignment_court_role");

                entity.HasIndex(e => e.CourtroomId)
                    .HasName("ix_assignment_courtroom");

                entity.HasIndex(e => e.EscortRunId)
                    .HasName("ix_assignment_escort_run");

                entity.HasIndex(e => e.JailRoleId)
                    .HasName("ix_assignment_jail_role");

                entity.HasIndex(e => e.LocationId)
                    .HasName("ix_assignment_location");

                entity.HasIndex(e => e.OtherAssignId)
                    .HasName("ix_assignment_other_assign_role");

                entity.HasIndex(e => e.WorkSectionCode)
                    .HasName("ix_assignment_work_section_code");

                entity.Property(e => e.AssignmentId)
                    .HasColumnName("assignment_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CourtRoleId).HasColumnName("court_role_id");

                entity.Property(e => e.CourtroomId).HasColumnName("courtroom_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.EscortRunId).HasColumnName("escort_run_id");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.JailRoleId).HasColumnName("jail_role_id");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.OtherAssignId).HasColumnName("other_assign_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasColumnType("numeric(3,0)");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.WorkSectionCode)
                    .IsRequired()
                    .HasColumnName("work_section_code")
                    .HasMaxLength(20);

                entity.HasOne(d => d.CourtRole)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.CourtRoleId)
                    .HasConstraintName("fk_court_role_id");

                entity.HasOne(d => d.Courtroom)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.CourtroomId)
                    .HasConstraintName("fk_courtroom_id");

                entity.HasOne(d => d.EscortRun)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.EscortRunId)
                    .HasConstraintName("fk_escort_run_id");

                entity.HasOne(d => d.JailRole)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.JailRoleId)
                    .HasConstraintName("fk_jail_role_id");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_location_id");

                entity.HasOne(d => d.OtherAssign)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.OtherAssignId)
                    .HasConstraintName("fk_other_assign_id");
            });

            modelBuilder.Entity<AudSheriffDuty>(entity =>
            {
                entity.HasKey(e => new { e.SheriffDutyId, e.RevisionCount })
                    .HasName("pk_aud_shrdty");

                entity.ToTable("aud_sheriff_duty", "shersched");

                entity.Property(e => e.SheriffDutyId).HasColumnName("sheriff_duty_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.DutyId).HasColumnName("duty_id");

                entity.Property(e => e.EndDtm)
                    .HasColumnName("end_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.OperationCode)
                    .IsRequired()
                    .HasColumnName("operation_code")
                    .HasMaxLength(20);

                entity.Property(e => e.OperationDtm)
                    .HasColumnName("operation_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.OperationUserId).HasColumnName("operation_user_id");

                entity.Property(e => e.SheriffId).HasColumnName("sheriff_id");

                entity.Property(e => e.StartDtm)
                    .HasColumnName("start_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<AuthRole>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("pk_role");

                entity.ToTable("auth_role", "shersched");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.RoleCode)
                    .IsRequired()
                    .HasColumnName("role_code")
                    .HasMaxLength(128);

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasColumnName("role_name")
                    .HasMaxLength(128);

                entity.Property(e => e.SystemRoleInd).HasColumnName("system_role_ind");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<AuthRolePermission>(entity =>
            {
                entity.HasKey(e => e.RolePermissionId)
                    .HasName("pk_role_permission");

                entity.ToTable("auth_role_permission", "shersched");

                entity.Property(e => e.RolePermissionId)
                    .HasColumnName("role_permission_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ApiScopePermissionId).HasColumnName("api_scope_permission_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.FrontendScopePermissionId).HasColumnName("frontend_scope_permission_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.RoleApiScopeId).HasColumnName("role_api_scope_id");

                entity.Property(e => e.RoleFrontendScopeId).HasColumnName("role_frontend_scope_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AuthRolePermission)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("fk_role_id");
            });

            modelBuilder.Entity<AuthUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("pk_user");

                entity.ToTable("auth_user", "shersched");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.DefaultLocationId).HasColumnName("default_location_id");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasColumnName("display_name")
                    .HasMaxLength(100);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SheriffId).HasColumnName("sheriff_id");

                entity.Property(e => e.SiteminderId)
                    .HasColumnName("siteminder_id")
                    .HasMaxLength(100);

                entity.Property(e => e.SystemAccountInd).HasColumnName("system_account_ind");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.UserAuthId)
                    .HasColumnName("user_auth_id")
                    .HasMaxLength(100);

                entity.HasOne(d => d.Sheriff)
                    .WithMany(p => p.AuthUser)
                    .HasForeignKey(d => d.SheriffId)
                    .HasConstraintName("fk_sheriff_id");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.InverseUser)
                    .HasForeignKey<AuthUser>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_id");
            });

            modelBuilder.Entity<AuthUserRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleId)
                    .HasName("pk_user_role");

                entity.ToTable("auth_user_role", "shersched");

                entity.HasIndex(e => e.LocationId)
                    .HasName("ix_location");

                entity.HasIndex(e => e.RoleId)
                    .HasName("ix_role");

                entity.HasIndex(e => e.UserId)
                    .HasName("ix_user");

                entity.Property(e => e.UserRoleId)
                    .HasColumnName("user_role_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.AuthUserRole)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("fk_location_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AuthUserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_role_id");
            });

            modelBuilder.Entity<CourtRoleCode>(entity =>
            {
                entity.HasKey(e => e.CourtRoleId)
                    .HasName("pk_court_role_code");

                entity.ToTable("court_role_code", "shersched");

                entity.HasIndex(e => e.CourtRoleCode1)
                    .HasName("court_role_code_loc_isnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NULL)");

                entity.HasIndex(e => new { e.CourtRoleCode1, e.LocationId })
                    .HasName("court_role_code_loc_notnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NOT NULL)");

                entity.Property(e => e.CourtRoleId)
                    .HasColumnName("court_role_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CourtRoleCode1)
                    .HasColumnName("court_role_code")
                    .HasMaxLength(64);

                entity.Property(e => e.CourtRoleName)
                    .HasColumnName("court_role_name")
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasColumnType("numeric(3,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.CourtRoleCode)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("fk_location_id");
            });

            modelBuilder.Entity<Courtroom>(entity =>
            {
                entity.ToTable("courtroom", "shersched");

                entity.HasIndex(e => e.CourtroomCode)
                    .HasName("courtroom_loc_isnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NULL)");

                entity.HasIndex(e => e.CourtroomName)
                    .HasName("ix_courtroom_name");

                entity.HasIndex(e => new { e.CourtroomCode, e.LocationId })
                    .HasName("courtroom_loc_notnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NOT NULL)");

                entity.Property(e => e.CourtroomId)
                    .HasColumnName("courtroom_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CourtroomCode)
                    .HasColumnName("courtroom_code")
                    .HasMaxLength(64);

                entity.Property(e => e.CourtroomName)
                    .HasColumnName("courtroom_name")
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasColumnType("numeric(3,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Courtroom)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("fk_location_id");
            });

            modelBuilder.Entity<Databasechangelog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("databasechangelog");

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasColumnName("author")
                    .HasMaxLength(255);

                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasMaxLength(255);

                entity.Property(e => e.Contexts)
                    .HasColumnName("contexts")
                    .HasMaxLength(255);

                entity.Property(e => e.Dateexecuted).HasColumnName("dateexecuted");

                entity.Property(e => e.DeploymentId)
                    .HasColumnName("deployment_id")
                    .HasMaxLength(10);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(e => e.Exectype)
                    .IsRequired()
                    .HasColumnName("exectype")
                    .HasMaxLength(10);

                entity.Property(e => e.Filename)
                    .IsRequired()
                    .HasColumnName("filename")
                    .HasMaxLength(255);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasColumnName("id")
                    .HasMaxLength(255);

                entity.Property(e => e.Labels)
                    .HasColumnName("labels")
                    .HasMaxLength(255);

                entity.Property(e => e.Liquibase)
                    .HasColumnName("liquibase")
                    .HasMaxLength(20);

                entity.Property(e => e.Md5sum)
                    .HasColumnName("md5sum")
                    .HasMaxLength(35);

                entity.Property(e => e.Orderexecuted).HasColumnName("orderexecuted");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Databasechangeloglock>(entity =>
            {
                entity.ToTable("databasechangeloglock");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Locked).HasColumnName("locked");

                entity.Property(e => e.Lockedby)
                    .HasColumnName("lockedby")
                    .HasMaxLength(255);

                entity.Property(e => e.Lockgranted).HasColumnName("lockgranted");
            });

            modelBuilder.Entity<Duty>(entity =>
            {
                entity.ToTable("duty", "shersched");

                entity.HasIndex(e => e.AssignmentId)
                    .HasName("ix_dty_asn");

                entity.HasIndex(e => e.DutyRecurrenceId)
                    .HasName("ix_dty_dtyrc");

                entity.Property(e => e.DutyId)
                    .HasColumnName("duty_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.DutyRecurrenceId).HasColumnName("duty_recurrence_id");

                entity.Property(e => e.EndDtm)
                    .HasColumnName("end_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.StartDtm)
                    .HasColumnName("start_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.Duty)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_dty_asn");

                entity.HasOne(d => d.DutyRecurrence)
                    .WithMany(p => p.Duty)
                    .HasForeignKey(d => d.DutyRecurrenceId)
                    .HasConstraintName("fk_dty_dtyrc");
            });

            modelBuilder.Entity<DutyRecurrence>(entity =>
            {
                entity.ToTable("duty_recurrence", "shersched");

                entity.HasIndex(e => e.AssignmentId)
                    .HasName("ix_dtyrc_asn");

                entity.Property(e => e.DutyRecurrenceId)
                    .HasColumnName("duty_recurrence_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.DaysBitmap)
                    .HasColumnName("days_bitmap")
                    .HasColumnType("numeric(10,0)")
                    .HasComment("1=mo,2=tu,4=we,8=th,16=fr,32=sa,64=su");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.EndTime)
                    .HasColumnName("end_time")
                    .HasColumnType("time with time zone");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SheriffsRequired)
                    .HasColumnName("sheriffs_required")
                    .HasColumnType("numeric(2,0)");

                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time")
                    .HasColumnType("time with time zone");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.DutyRecurrence)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_dtyrc_asn");
            });

            modelBuilder.Entity<EscortRun>(entity =>
            {
                entity.ToTable("escort_run", "shersched");

                entity.HasIndex(e => e.EscortRunCode)
                    .HasName("escort_run_loc_isnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NULL)");

                entity.HasIndex(e => new { e.EscortRunCode, e.LocationId })
                    .HasName("escort_run_loc_notnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NOT NULL)");

                entity.Property(e => e.EscortRunId)
                    .HasColumnName("escort_run_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.EscortRunCode)
                    .HasColumnName("escort_run_code")
                    .HasMaxLength(64);

                entity.Property(e => e.EscortRunName)
                    .HasColumnName("escort_run_name")
                    .HasMaxLength(128);

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasColumnType("numeric(3,0)");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(128);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.EscortRun)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("fk_location_id");
            });

            modelBuilder.Entity<GenderCode>(entity =>
            {
                entity.HasKey(e => e.GenderCode1)
                    .HasName("pk_gndr");

                entity.ToTable("gender_code", "shersched");

                entity.HasComment("Gender Code captures the standard system values for gender     Male     Female     Other");

                entity.Property(e => e.GenderCode1)
                    .HasColumnName("gender_code")
                    .HasMaxLength(10);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<JailRoleCode>(entity =>
            {
                entity.HasKey(e => e.JailRoleId)
                    .HasName("pk_jail_role_code");

                entity.ToTable("jail_role_code", "shersched");

                entity.HasIndex(e => e.JailRoleCode1)
                    .HasName("jail_role_code_loc_isnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NULL)");

                entity.HasIndex(e => new { e.JailRoleCode1, e.LocationId })
                    .HasName("jail_role_code_loc_notnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NOT NULL)");

                entity.Property(e => e.JailRoleId)
                    .HasColumnName("jail_role_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.JailRoleCode1)
                    .HasColumnName("jail_role_code")
                    .HasMaxLength(64);

                entity.Property(e => e.JailRoleName)
                    .HasColumnName("jail_role_name")
                    .HasMaxLength(128);

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasColumnType("numeric(3,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.JailRoleCode)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("fk_location_id");
            });

            modelBuilder.Entity<Leave>(entity =>
            {
                entity.ToTable("leave", "shersched");

                entity.HasComment("Leave captures data related to absence from regular duty for one or more full days.");

                entity.Property(e => e.LeaveId)
                    .HasColumnName("leave_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CancelledDtm)
                    .HasColumnName("cancelled_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("date");

                entity.Property(e => e.EndTime)
                    .HasColumnName("end_time")
                    .HasColumnType("time with time zone");

                entity.Property(e => e.LeaveCancelReasonCode)
                    .HasColumnName("leave_cancel_reason_code")
                    .HasMaxLength(20);

                entity.Property(e => e.LeaveCode)
                    .IsRequired()
                    .HasColumnName("leave_code")
                    .HasMaxLength(20);

                entity.Property(e => e.LeaveSubCode)
                    .IsRequired()
                    .HasColumnName("leave_sub_code")
                    .HasMaxLength(20);

                entity.Property(e => e.PartialDayInd).HasColumnName("partial_day_ind");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SheriffId).HasColumnName("sheriff_id");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("date");

                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time")
                    .HasColumnType("time with time zone");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.LeaveCancelReasonCodeNavigation)
                    .WithMany(p => p.Leave)
                    .HasForeignKey(d => d.LeaveCancelReasonCode)
                    .HasConstraintName("fk_lve_lvcr");

                entity.HasOne(d => d.Sheriff)
                    .WithMany(p => p.Leave)
                    .HasForeignKey(d => d.SheriffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_lve_shrf");

                entity.HasOne(d => d.LeaveNavigation)
                    .WithMany(p => p.Leave)
                    .HasForeignKey(d => new { d.LeaveCode, d.LeaveSubCode })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_lve_lvsbcd");
            });

            modelBuilder.Entity<LeaveCancelReasonCode>(entity =>
            {
                entity.HasKey(e => e.LeaveCancelReasonCode1)
                    .HasName("pk_lvcr");

                entity.ToTable("leave_cancel_reason_code", "shersched");

                entity.HasComment("Leave Cancel Reason Code captures the reasons for cancellation of leave, for BI and auditing purposes. Initial values are     - Operational Demands     - Personal Decision    - Entry Error");

                entity.Property(e => e.LeaveCancelReasonCode1)
                    .HasColumnName("leave_cancel_reason_code")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<LeaveCode>(entity =>
            {
                entity.HasKey(e => e.LeaveCode1)
                    .HasName("pk_lvcd");

                entity.ToTable("leave_code", "shersched");

                entity.HasComment("Leave Type Code captures the different categories of leave being managed. Initial types are     Leave     Training");

                entity.Property(e => e.LeaveCode1)
                    .HasColumnName("leave_code")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<LeaveSubCode>(entity =>
            {
                entity.HasKey(e => new { e.LeaveCode, e.LeaveSubCode1 })
                    .HasName("pk_lvsbcd");

                entity.ToTable("leave_sub_code", "shersched");

                entity.HasComment("Leave Type Sub Code captures the different types of leave being managed in each category. Initial types are     Leave -     STIP (Short Term Illness)     Leave -     Annual Leave     Leave -     Special Leave     Training -  Training");

                entity.Property(e => e.LeaveCode)
                    .HasColumnName("leave_code")
                    .HasMaxLength(20);

                entity.Property(e => e.LeaveSubCode1)
                    .HasColumnName("leave_sub_code")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasColumnType("numeric(3,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.LeaveCodeNavigation)
                    .WithMany(p => p.LeaveSubCode)
                    .HasForeignKey(d => d.LeaveCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_lvsbcd_lvcd");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("location", "shersched");

                entity.HasIndex(e => e.LocationCd)
                    .HasName("uk_locn_cd")
                    .IsUnique();

                entity.HasIndex(e => e.ParentLocationId)
                    .HasName("ix_locn_prt");

                entity.Property(e => e.LocationId)
                    .HasColumnName("location_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.JustinCode)
                    .HasColumnName("justin_code")
                    .HasMaxLength(10);

                entity.Property(e => e.JustinId)
                    .HasColumnName("justin_id")
                    .HasMaxLength(20);

                entity.Property(e => e.LocationCd)
                    .IsRequired()
                    .HasColumnName("location_cd")
                    .HasMaxLength(5)
                    .HasComment("business key");

                entity.Property(e => e.LocationName)
                    .IsRequired()
                    .HasColumnName("location_name")
                    .HasMaxLength(100);

                entity.Property(e => e.ParentLocationId).HasColumnName("parent_location_id");

                entity.Property(e => e.RegionId).HasColumnName("region_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.LocationNavigation)
                    .HasForeignKey(d => d.RegionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_locn_reg");
            });

            modelBuilder.Entity<OtherAssignCode>(entity =>
            {
                entity.HasKey(e => e.OtherAssignId)
                    .HasName("pk_other_assign_code");

                entity.ToTable("other_assign_code", "shersched");

                entity.HasIndex(e => e.OtherAssignCode1)
                    .HasName("other_assign_code_loc_isnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NULL)");

                entity.HasIndex(e => new { e.OtherAssignCode1, e.LocationId })
                    .HasName("other_assign_code_loc_notnull_idx")
                    .IsUnique()
                    .HasFilter("(location_id IS NOT NULL)");

                entity.Property(e => e.OtherAssignId)
                    .HasColumnName("other_assign_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.OtherAssignCode1)
                    .HasColumnName("other_assign_code")
                    .HasMaxLength(64);

                entity.Property(e => e.OtherAssignName)
                    .HasColumnName("other_assign_name")
                    .HasMaxLength(128);

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasColumnType("numeric(3,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.OtherAssignCode)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("fk_location_id");
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.ToTable("region", "shersched");

                entity.HasIndex(e => e.RegionCd)
                    .HasName("uk_rgn_cd")
                    .IsUnique();

                entity.Property(e => e.RegionId)
                    .HasColumnName("region_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Location).HasColumnName("location");

                entity.Property(e => e.RegionCd)
                    .IsRequired()
                    .HasColumnName("region_cd")
                    .HasMaxLength(12)
                    .HasComment("business key");

                entity.Property(e => e.RegionName)
                    .IsRequired()
                    .HasColumnName("region_name")
                    .HasMaxLength(100);

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<Sheriff>(entity =>
            {
                entity.ToTable("sheriff", "shersched");

                entity.HasIndex(e => e.BadgeNo)
                    .HasName("uk_shrf_bdgn")
                    .IsUnique();

                entity.HasIndex(e => e.CurrentLocationId)
                    .HasName("ix_shr_crlocn");

                entity.HasIndex(e => e.HomeLocationId)
                    .HasName("ix_shr_hmlocn");

                entity.HasIndex(e => e.SheriffRankCode)
                    .HasName("ix_shr_rkcd");

                entity.Property(e => e.SheriffId)
                    .HasColumnName("sheriff_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Alias)
                    .HasColumnName("alias")
                    .HasMaxLength(32);

                entity.Property(e => e.BadgeNo)
                    .IsRequired()
                    .HasColumnName("badge_no")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.CurrentLocationId).HasColumnName("current_location_id");

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(100);

                entity.Property(e => e.GenderCode)
                    .HasColumnName("gender_code")
                    .HasMaxLength(10);

                entity.Property(e => e.HomeLocationId)
                    .HasColumnName("home_location_id")
                    .HasComment("Permanent location for a sheriff");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("image_url")
                    .HasMaxLength(200)
                    .HasComment("TBD how to store a Sheriff photo");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(100);

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SheriffRankCode)
                    .IsRequired()
                    .HasColumnName("sheriff_rank_code")
                    .HasMaxLength(20);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.CurrentLocation)
                    .WithMany(p => p.SheriffCurrentLocation)
                    .HasForeignKey(d => d.CurrentLocationId)
                    .HasConstraintName("fk_shr_crlocn");

                entity.HasOne(d => d.GenderCodeNavigation)
                    .WithMany(p => p.Sheriff)
                    .HasForeignKey(d => d.GenderCode)
                    .HasConstraintName("fk_shrf_gndr");

                entity.HasOne(d => d.HomeLocation)
                    .WithMany(p => p.SheriffHomeLocation)
                    .HasForeignKey(d => d.HomeLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_shr_hmlocn");

                entity.HasOne(d => d.SheriffRankCodeNavigation)
                    .WithMany(p => p.Sheriff)
                    .HasForeignKey(d => d.SheriffRankCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_shr_shrkcd");
            });

            modelBuilder.Entity<SheriffDuty>(entity =>
            {
                entity.ToTable("sheriff_duty", "shersched");

                entity.HasIndex(e => e.DutyId)
                    .HasName("ix_shrdty_dty");

                entity.HasIndex(e => e.SheriffId)
                    .HasName("ix_shrdty_shr");

                entity.Property(e => e.SheriffDutyId)
                    .HasColumnName("sheriff_duty_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.DutyId).HasColumnName("duty_id");

                entity.Property(e => e.EndDtm)
                    .HasColumnName("end_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SheriffId).HasColumnName("sheriff_id");

                entity.Property(e => e.StartDtm)
                    .HasColumnName("start_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Duty)
                    .WithMany(p => p.SheriffDuty)
                    .HasForeignKey(d => d.DutyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_shrdty_dty");

                entity.HasOne(d => d.Sheriff)
                    .WithMany(p => p.SheriffDuty)
                    .HasForeignKey(d => d.SheriffId)
                    .HasConstraintName("fk_shrdty_shr");
            });

            modelBuilder.Entity<SheriffLocation>(entity =>
            {
                entity.ToTable("sheriff_location", "shersched");

                entity.Property(e => e.SheriffLocationId)
                    .HasColumnName("sheriff_location_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("date");

                entity.Property(e => e.EndTime)
                    .HasColumnName("end_time")
                    .HasColumnType("time without time zone");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.PartialDayInd).HasColumnName("partial_day_ind");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SheriffId).HasColumnName("sheriff_id");

                entity.Property(e => e.SortOrder)
                    .HasColumnName("sort_order")
                    .HasColumnType("numeric(3,0)");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("date");

                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time")
                    .HasColumnType("time without time zone");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.SheriffLocation)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("fk_location_id");

                entity.HasOne(d => d.Sheriff)
                    .WithMany(p => p.SheriffLocation)
                    .HasForeignKey(d => d.SheriffId)
                    .HasConstraintName("fk_sheriff_id");
            });

            modelBuilder.Entity<SheriffRankCode>(entity =>
            {
                entity.HasKey(e => e.SheriffRankCode1)
                    .HasName("pk_shrkcd");

                entity.ToTable("sheriff_rank_code", "shersched");

                entity.Property(e => e.SheriffRankCode1)
                    .HasColumnName("sheriff_rank_code")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.RankOrder).HasColumnName("rank_order");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.ToTable("shift", "shersched");

                entity.HasIndex(e => e.LocationId)
                    .HasName("ix_shft_locn");

                entity.HasIndex(e => e.SheriffId)
                    .HasName("ix_shft_shr");

                entity.HasIndex(e => e.WorkSectionCode)
                    .HasName("ix_shft_wsc");

                entity.Property(e => e.ShiftId)
                    .HasColumnName("shift_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.EndDtm)
                    .HasColumnName("end_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.SheriffId).HasColumnName("sheriff_id");

                entity.Property(e => e.StartDtm)
                    .HasColumnName("start_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.WorkSectionCode)
                    .HasColumnName("work_section_code")
                    .HasMaxLength(20);

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.Shift)
                    .HasForeignKey(d => d.AssignmentId)
                    .HasConstraintName("fk_shft_asn");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Shift)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_shft_locn");

                entity.HasOne(d => d.Sheriff)
                    .WithMany(p => p.Shift)
                    .HasForeignKey(d => d.SheriffId)
                    .HasConstraintName("fk_shft_shr");

                entity.HasOne(d => d.WorkSectionCodeNavigation)
                    .WithMany(p => p.Shift)
                    .HasForeignKey(d => d.WorkSectionCode)
                    .HasConstraintName("fk_shft_wsc");
            });

            modelBuilder.Entity<WorkSectionCode>(entity =>
            {
                entity.HasKey(e => e.WorkSectionCode1)
                    .HasName("pk_wksccd");

                entity.ToTable("work_section_code", "shersched");

                entity.Property(e => e.WorkSectionCode1)
                    .HasColumnName("work_section_code")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasMaxLength(32);

                entity.Property(e => e.CreatedDtm)
                    .HasColumnName("created_dtm")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effective_date")
                    .HasColumnType("date");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("date");

                entity.Property(e => e.RevisionCount)
                    .HasColumnName("revision_count")
                    .HasColumnType("numeric(10,0)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasMaxLength(32);

                entity.Property(e => e.UpdatedDtm)
                    .HasColumnName("updated_dtm")
                    .HasColumnType("timestamp with time zone");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
