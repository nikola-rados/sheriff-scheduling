using Mapster;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SS.Common.attributes.mapping;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto", IgnoreAttributes = new[] { typeof(JsonIgnoreAttribute) })]
    [GenerateUpdateDto, GenerateAddDto]
    public class SheriffRank
    {
        [ExcludeFromAddDto]
        [Key]
        public int Id { get; set; }
        public Guid SheriffId { get; set; }
        [ExcludeFromAddAndUpdateDto]
        [AdaptIgnore]
        public virtual Sheriff Sheriff { get; set; }
        public string Rank { get;set;}
        public DateTimeOffset EffectiveDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}
