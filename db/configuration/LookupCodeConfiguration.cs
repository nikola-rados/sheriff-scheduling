using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using ss.db.models;
using SS.Db.models.auth;
using SS.Db.models.lookupcodes;

namespace SS.Db.configuration
{
    public class LookupCodeOrderConfiguration : BaseEntityConfiguration<LookupCode>
    {
        public override void Configure(EntityTypeBuilder<LookupCode> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 1000);

            builder.HasIndex(lc => new {lc.Type, lc.Code, lc.LocationId}).IsUnique();

            builder.HasOne(b => b.Location).WithMany().HasForeignKey(lc => lc.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasData(
                new LookupCode { CreatedById = User.SystemUser, Id = 1, Description = "Chief Sheriff", Type = LookupTypes.SheriffRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 2, Description = "Superintendent", Type = LookupTypes.SheriffRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 3, Description = "Staff Inspector", Type = LookupTypes.SheriffRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 4, Description = "Inspector", Type = LookupTypes.SheriffRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 5, Description = "Staff Sergeant", Type = LookupTypes.SheriffRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 6, Description = "Sergeant", Type = LookupTypes.SheriffRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 7, Description = "Deputy Sheriff", Type = LookupTypes.SheriffRank },
              
                new LookupCode { CreatedById = User.SystemUser, Id = 8, Description = "CEW (Taser)", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 9, Description = "DNA", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 10, Description = "FRO", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 11, Description = "Fire Arm", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 12, Description = "First Aid", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 13, Description = "Advanced Escort SPC (AESOC)", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 14, Description = "Extenuating Circumstances SPC (EXSPC)", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 15, Description = "Search Gate", Type = LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 16, Description = "Other", Type = LookupTypes.TrainingType },
                
                new LookupCode { CreatedById = User.SystemUser, Id = 17, Description = "STIP", Type = LookupTypes.LeaveType},
                new LookupCode { CreatedById = User.SystemUser, Id = 18, Description = "Annual", Type = LookupTypes.LeaveType },
                new LookupCode { CreatedById = User.SystemUser, Id = 19, Description = "Illness", Type = LookupTypes.LeaveType },
                new LookupCode { CreatedById = User.SystemUser, Id = 20, Description = "Special", Type = LookupTypes.LeaveType }
            );
            base.Configure(builder);
        }
    }
}
