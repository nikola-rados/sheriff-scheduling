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

            builder.HasMany(m => m.UserRoles).WithOne(m => m.User).HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.ClientCascade);

            builder.HasOne(l => l.HomeLocation).WithMany().HasForeignKey(m => m.HomeLocationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasData(
                new User { Id = User.SystemUser, FirstName = "SYSTEM", LastName = "SYSTEM", IsEnabled = false }
            );

            base.Configure(builder);
        }
    }
}
