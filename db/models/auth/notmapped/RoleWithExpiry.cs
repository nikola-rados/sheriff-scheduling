using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SS.Db.models.auth.notmapped
{
    public class RoleWithExpiry
    {
        [NotMapped]
        public Role Role { get; set; }
        [NotMapped]
        public DateTimeOffset EffectiveDate { get; set; }
        [NotMapped]
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}
