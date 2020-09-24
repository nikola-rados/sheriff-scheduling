using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;

namespace SS.Db.models.auth
{
    public class Permission : BaseEntity
    {
        public const string Login = nameof(Login);
        public const string IsAdmin = nameof(IsAdmin);

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
