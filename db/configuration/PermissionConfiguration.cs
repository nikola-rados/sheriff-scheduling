using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.Db.models.auth;
using SS.DB.Configuration;

namespace SS.Db.configuration
{
    public class PermissionConfiguration : BaseEntityConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 50);

            builder.HasData(
                new Permission { Id = 1, Name = Permission.Login, Description = "Permission to login to the application"}
            );
            base.Configure(builder);
        }
    }
}
