using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.sheriff;

namespace SS.Db.configuration
{
    public class SheriffConfiguration : BaseEntityConfiguration<Sheriff>
    {
        public override void Configure(EntityTypeBuilder<Sheriff> builder)
        {
            builder.HasIndex(s => s.BadgeNumber).IsUnique();

            base.Configure(builder);
        }
    }
}
