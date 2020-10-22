using SS.DB.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.Api.Models.DB;
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

            builder.HasMany(b => b.AssignedSheriffs).WithOne(s => s.Shift).HasForeignKey(m => m.ShiftId)
                .OnDelete(DeleteBehavior.SetNull);

            base.Configure(builder);
        }
    }
}
