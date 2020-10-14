
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using db.models;
using Mapster;

namespace SS.Db.models.auth
{
    [AdaptTo("[name]Dto")]
    public class Permission : BaseEntity
    {
        public const string Login = nameof(Login);
        public const string ViewOwnProfile = nameof(ViewOwnProfile);
        public const string ViewProfilesInOwnLocation = nameof(ViewProfilesInOwnLocation);
        public const string ViewProfilesInAllLocation = nameof(ViewProfilesInAllLocation);
        public const string CreateUsers = nameof(CreateUsers);
        public const string ExpireUsers = nameof(ExpireUsers);
        public const string EditUsers = nameof(EditUsers);
        public const string ViewRoles = nameof(ViewRoles);
        public const string CreateAndAssignRoles = nameof(CreateAndAssignRoles);
        public const string ExpireRoles = nameof(ExpireRoles);
        public const string EditRoles = nameof(EditRoles);
        public const string ViewManageTypes = nameof(ViewManageTypes);
        public const string CreateTypes = nameof(CreateTypes);
        public const string EditTypes = nameof(EditTypes);
        public const string ExpireTypes = nameof(ExpireTypes);
        public const string ViewMyShifts = nameof(ViewMyShifts);
        public const string ViewAllShiftsAtMyLocation = nameof(ViewAllShiftsAtMyLocation);
        public const string ViewAllShifts = nameof(ViewAllShifts);
        public const string CreateAndAssignShifts = nameof(CreateAndAssignShifts);
        public const string ExpireShifts = nameof(ExpireShifts);
        public const string EditShifts = nameof(EditShifts);
        public const string ViewDistributeSchedule = nameof(ViewDistributeSchedule);
        public const string ViewHomeLocationAndAssignedLocation = nameof(ViewHomeLocationAndAssignedLocation);
        public const string ViewRegion = nameof(ViewRegion);
        public const string ViewProvince = nameof(ViewProvince);
        public const string ExpireLocation = nameof(ExpireLocation);

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
