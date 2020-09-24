using SS.Api.models.db;
using System.ComponentModel.DataAnnotations;
using db.models;

namespace SS.Api.Models.DB
{
    public class Location : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int JustinId { get; set; }
        public string JustinCode { get; set; }
        public int ParentLocationId { get; set; }
        public Region Region { get; set; }
    }
}
