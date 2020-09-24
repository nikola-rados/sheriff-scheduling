using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.exceptions;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.services
{
    /// <summary>
    /// This should handle all the permissions / roles / base user updates. 
    /// </summary>
    public class AuthService
    {
        private readonly SheriffDbContext _db;
        public AuthService(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<User> UpsertUserFromClaims(ClaimsIdentity claimsIdentity)
        {
            var claims = claimsIdentity.Claims.ToList();
            var id = Guid.Parse(claims.GetValueByType(ClaimTypes.NameIdentifier));
            var user = await _db.User.FindAsync(id);
            if (user != null && !user.IsDisabled)
            {
                user.LastLogin = DateTime.Now;
            }
            else if (user == null)
            {
                user = new User
                {
                    Id = id,
                    PreferredUsername = claims.GetValueByType("preferred_username"),
                    IdirId = Guid.Parse(claims.GetValueByType("idir_userid")),
                    FirstName = claims.GetValueByType(ClaimTypes.GivenName),
                    LastName = claims.GetValueByType(ClaimTypes.Name),
                    Email = claims.GetValueByType(ClaimTypes.Email),
                    IsDisabled = true
                };
                await _db.User.AddAsync(user);
            }
            await _db.SaveChangesAsync();
            return user;
        }

        #region Permissions
        public async Task AssignPermissionToRole(int roleId, int permissionId)
        {
            var role = await _db.Role.FindAsync(roleId);
            if (role == null)
                throw new BusinessLayerException($"Role with id {roleId} does not exist.");

            var permission = await _db.Permission.FindAsync(permissionId);
            if (permission == null)
                throw new BusinessLayerException($"Permission with id {permissionId} does not exist.");

            if (role.RolePermissions.Any(rp => rp.Role.Id == roleId && rp.Permission.Id == permissionId))
                return;

            var rolePermission = new RolePermission
            {
                CreatedById = new Guid(), 
                CreatedOn = DateTime.UtcNow,
                Permission = permission,
                Role = role
            };

            role.RolePermissions.Add(rolePermission);

            await _db.SaveChangesAsync();
        }

        public async Task UnassignPermissionFromRole(int roleId, int permissionId)
        {
            var role = await _db.Role.FindAsync(roleId);
            if (role == null)
                throw new BusinessLayerException($"Role with id {roleId} does not exist.");

            var userPermission = role.RolePermissions.FirstOrDefault(p => p.Permission.Id == permissionId && p.Role.Id == roleId);
            if (userPermission == null)
                throw new BusinessLayerException($"Permission with id {permissionId} does not exist.");

            role.RolePermissions.Remove(userPermission);
            await _db.SaveChangesAsync();
        }

        #endregion Permissions

        #region Roles
        public async Task AssignRole(Guid userId, int roleId)
        {
            var user = await _db.User.FindAsync(userId);
            if (user == null)
                throw new BusinessLayerException($"User with id {userId} does not exist.");

            var role = await _db.Role.FindAsync(roleId);
            if (role == null)
                throw new BusinessLayerException($"Role with id {roleId} does not exist.");

            if (user.Roles.Any(r => r.UserId == userId && r.RoleId == roleId))
                return;

            user.Roles.Add(new UserRole
            {
                Role = role,
                User = user
            });

            await _db.SaveChangesAsync();
        }

        public async Task<int> CreateRole(Role role)
        {
            await _db.Role.AddAsync(role);
            await _db.SaveChangesAsync();
            return role.Id;
        }

        public async Task DeleteRole(int roleId)
        {
            var role = await _db.Role.FindAsync(roleId);
            _db.Role.Remove(role);
            await _db.SaveChangesAsync();
        }
        
        public async Task UnassignRole(Guid userId, int roleId)
        {
            var user = await _db.User.FindAsync(userId);
            if (user == null)
                throw new BusinessLayerException($"User with id {userId} does not exist.");

            var userRole = user.Roles.FirstOrDefault(r => r.UserId == userId && r.RoleId == roleId);
            if (userRole == null)
                throw new BusinessLayerException($"UserRole with Userid: {userId}, RoleId: {roleId} does not exist.");

            user.Roles.Remove(userRole);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateRole(Role role)
        {
            _db.Role.Update(role);
            await _db.SaveChangesAsync();
        }

        #endregion Roles
    }
}
