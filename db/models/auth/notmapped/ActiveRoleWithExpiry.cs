using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Mapster;
using Newtonsoft.Json;

namespace SS.Db.models.auth.notmapped
{
    /// <summary>
    /// Needed two separate classes, so DTO generation would work correctly. 
    /// </summary>
    [AdaptTo("[name]Dto")]
    public class ActiveRoleWithExpiry
    {
        [NotMapped]
        public Role Role
        {
            get
            {
                _role.RolePermissions = null;
                return _role;
            }
            set => _role = value;
        }

        private Role _role;

        [NotMapped]
        public DateTimeOffset EffectiveDate { get; set; }
        [NotMapped]
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}
