using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.sheriff;

namespace SS.Db.configuration
{
    public class SheriffAwayLocationConfiguration : BaseEntityConfiguration<SheriffAwayLocation>
    {
        public override void Configure(EntityTypeBuilder<SheriffAwayLocation> builder)
        {
            builder.HasOne(b => b.Location).WithMany().HasForeignKey(m => m.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => new {b.StartDate, b.EndDate});

            base.Configure(builder);
        }
    }
}
