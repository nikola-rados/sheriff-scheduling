using System;
using System.ComponentModel.DataAnnotations;
using db.models;

namespace SS.Db.models.auth
{
    public class UserRole : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
