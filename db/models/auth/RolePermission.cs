using System.ComponentModel.DataAnnotations;
using db.models;

namespace SS.Db.models.auth
{
    public class RolePermission : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}
