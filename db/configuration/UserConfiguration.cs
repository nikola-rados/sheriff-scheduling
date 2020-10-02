using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.auth;

namespace SS.Db.configuration
{
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            //builder.HasMany(m => m.Roles).WithOne(m => m.User).HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.ClientCascade);
            //builder.HasMany(m => m.Permissions).WithOne(m => m.Id).HasForeignKey(m => m.RoleId).OnDelete(DeleteBehavior.ClientCascade);

            base.Configure(builder);
        }
    }
}
