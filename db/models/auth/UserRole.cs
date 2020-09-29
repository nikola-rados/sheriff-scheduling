using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;

namespace SS.Db.models.auth
{
    public class UserRole : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        [AdaptIgnore]
        public virtual User User { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
