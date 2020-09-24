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
            builder.HasOne(m => m.User).WithMany(m => m.Roles).HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(m => m.Role).WithMany(m => m.Users).HasForeignKey(m => m.RoleId).OnDelete(DeleteBehavior.ClientCascade);

            base.Configure(builder);
        }
    }
}
