using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.scheduling;

namespace SS.Db.configuration
{
    public class DutyConfiguration : BaseEntityConfiguration<Duty>
    {
        public override void Configure(EntityTypeBuilder<Duty> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasOne(d => d.Assignment).WithMany().OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(d => d.Location).WithMany().OnDelete(DeleteBehavior.SetNull);
            builder.HasMany(d => d.DutySlots).WithOne().HasForeignKey(m => m.DutyId).OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
