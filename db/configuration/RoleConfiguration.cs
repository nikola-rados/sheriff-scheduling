using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.auth;

namespace SS.Db.configuration
{
    public class RoleConfiguration : BaseEntityConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 50);

            builder.HasData(
                new Role { Id = 1, Name = Role.Administrator, Description = "Administrator" },
                new Role { Id = 2, Name = Role.Manager, Description = "Manager" },
                new Role { Id = 3, Name = Role.Sheriff, Description = "Sheriff" }
            );

            base.Configure(builder);
        }
    }
}
