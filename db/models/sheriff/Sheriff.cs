using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mapster;
using Newtonsoft.Json;
using SS.Db.models.auth;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto", IgnoreAttributes = new [] {typeof(JsonIgnoreAttribute)})]
    public class Sheriff : User
    {
        public Gender Gender { get; set; }
        public string BadgeNumber { get; set; }
        public virtual List<SheriffAwayLocation> AwayLocation { get; set; } = new List<SheriffAwayLocation>();
        public virtual List<SheriffLeave> Leave { get; set; } = new List<SheriffLeave>();
        public virtual List<SheriffTraining> Training { get; set; } = new List<SheriffTraining>();
        public virtual List<SheriffRank> Rank { get; set; } = new List<SheriffRank>();
        [AdaptIgnore]
        public byte[] Photo { get; set; }
        [NotMapped] 
        public string PhotoUrl => Photo?.Length > 0 ? $"/api/sheriff/getPhoto/{Id}?{LastPhotoUpdate.Ticks}" : null;
        public DateTimeOffset LastPhotoUpdate { get; set; }
    }
}
