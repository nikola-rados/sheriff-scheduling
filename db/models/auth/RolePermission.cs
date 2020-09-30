using System.ComponentModel.DataAnnotations;
using db.models;

namespace SS.Db.models.auth
{
    public class RolePermission : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public virtual Role Role { get; set; }
        public int RoleId { get; set; }
        public virtual Permission Permission { get; set; }
        public int PermissionId { get; set; }
    }
}
