using db.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;

namespace SS.Db.configuration
{
    public class RegionConfiguration : BaseEntityConfiguration<Region>
    {
        public override void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasIndex(b => b.JustinId).IsUnique();

            base.Configure(builder);
        }
    }
}
