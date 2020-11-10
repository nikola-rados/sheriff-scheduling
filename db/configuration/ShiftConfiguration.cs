using SS.DB.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.Db.models.scheduling;

namespace SS.Db.configuration
{
    public class ShiftConfiguration : BaseEntityConfiguration<Shift>
    {
        public override void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasOne(b => b.Location).WithMany().HasForeignKey(m => m.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.AnticipatedAssignment).WithMany().OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.Sheriff).WithMany().OnDelete(DeleteBehavior.SetNull);

            base.Configure(builder);
        }
    }
}
