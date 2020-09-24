using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;

namespace SS.Db.models.auth
{
    public class Role : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        public virtual ICollection<UserRole> Users { get; set; }
    }
}
