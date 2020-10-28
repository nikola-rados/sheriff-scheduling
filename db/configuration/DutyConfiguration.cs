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

            builder.HasOne(d => d.Assignment).WithOne().OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(d => d.Location).WithOne().OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(d => d.Shift).WithMany(s => s.Duties).OnDelete(DeleteBehavior.SetNull);

            base.Configure(builder);
        }
    }
}
