using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.Api.Models.DB;
using SS.DB.Configuration;

namespace SS.Db.configuration
{
    public class LocationConfiguration : BaseEntityConfiguration<Location>
    {

        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasData(
                new Location { Id = -1, Name = "Dummy Location", AgencyId = "FAKE" },
                new Location { Id = -2, Name = "Dummy Location2", AgencyId = "FAKE2" }
            );

            builder.HasIndex(b => b.AgencyId).IsUnique();

            base.Configure(builder);
        }
    }
}
