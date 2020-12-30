using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.scheduling;

namespace SS.Db.configuration
{
    public class DutySlotConfiguration : BaseEntityConfiguration<DutySlot>
    {
        public override void Configure(EntityTypeBuilder<DutySlot> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasOne(d => d.Sheriff).WithMany().OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(d => d.Location).WithMany().OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(d => d.Duty).WithMany(d => d.DutySlots).HasForeignKey(d => d.DutyId)
                .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
