using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using db.models;
using Mapster;
using SS.Api.Models.DB;

namespace SS.Db.models.auth
{
    public class User : BaseEntity
    {
        public User()
        {
            CreatedOn = DateTime.Now;
        }
        [Key]
        public Guid Id { get; set; }
        public string IdirName { get; set; }
        [AdaptIgnore]
        public Guid? IdirId { get; set; }
        [AdaptIgnore]
        public Guid? KeyCloakId { get; set; }
        [AdaptIgnore]
        public bool IsEnabled { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? HomeLocationId { get; set; }
        [ForeignKey("HomeLocationId")]
        public virtual Location HomeLocation { get; set; }
        [AdaptIgnore]
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        [AdaptIgnore]
        [NotMapped]
        public virtual ICollection<Permission> Permissions
        {
            get
            {
                return UserRoles.
                    SelectMany(x => x.Role.RolePermissions).Select(x => x.Permission).Distinct().ToList();
            }
        }
        [AdaptIgnore]
        public DateTime? LastLogin { get; set; }
    }
}
