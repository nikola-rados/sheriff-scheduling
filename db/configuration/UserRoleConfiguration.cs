using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.auth;

namespace SS.Db.configuration
{
    public class UserRoleConfiguration : BaseEntityConfiguration<UserRole>
    {
        public override void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 100);

            builder.HasIndex(lc => new { lc.RoleId, lc.UserId }).IsUnique();

            builder.HasOne(m => m.User).WithMany(m => m.UserRoles).HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(m => m.Role).WithMany(m => m.UserRoles).HasForeignKey(m => m.RoleId).OnDelete(DeleteBehavior.ClientCascade);

            base.Configure(builder);
        }
    }
}
