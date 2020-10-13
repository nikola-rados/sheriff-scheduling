using System;
using System.ComponentModel.DataAnnotations.Schema;
using db.models;
using Mapster;
using Newtonsoft.Json;

namespace SS.Db.models.auth.notmapped
{
    /// <summary>
    /// Needed two separate classes, so DTO generation would work correctly. 
    /// </summary>
    [AdaptTo("[name]Dto")]
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
