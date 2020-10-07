using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.exceptions;
using SS.Db.models;
using SS.Db.models.auth;

namespace SS.Api.services
{
    public class RoleService
    {
        private readonly SheriffDbContext _db;

        public RoleService(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<List<Role>> Roles()
        {
            return await _db.Role.ToListAsync();
        }

        public async Task<Role> Role(int id)
        {
            return await _db.Role.Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission).SingleOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role> AddRole(Role role)
        {
            var roleAlreadyExistsWithName = await _db.Role.AnyAsync(r => r.Name == role.Name);
            if (roleAlreadyExistsWithName)
                throw new BusinessLayerException($"Role with name {role.Name} already exists.");

            role.RolePermissions = null;
            role.UserRoles = null;
            await _db.Role.AddAsync(role);
            await _db.SaveChangesAsync();
            return role;
        }

        public async Task RemoveRole(int roleId)
        {
            var role = await _db.Role.FindAsync(roleId);
            _db.Role.Remove(role);
            await _db.SaveChangesAsync();
        }

        public async Task<Role> UpdateRole(Role role)
        {
            var savedRole = await _db.Role.FindAsync(role.Id);
            if (savedRole.Name != role.Name)
            {
                var roleAlreadyExistsWithName = await _db.Role.AnyAsync(r => r.Name == role.Name);
                if (roleAlreadyExistsWithName)
                    throw new BusinessLayerException($"Role with name {role.Name} already exists.");
            }

            _db.Entry(savedRole).CurrentValues.SetValues(role);
            await _db.SaveChangesAsync();
            return role;
        }

        public async Task AssignPermissionsToRole(int roleId, List<int> permissionIds)
        {
            var role = await _db.Role.Include(r => r.RolePermissions)
                                     .FirstOrDefaultAsync( r=> r.Id == roleId);
            role.ThrowBusinessExceptionIfNull($"Role with id {roleId} does not exist.");

            foreach (var permissionId in permissionIds)
            {
                var permission = await _db.Permission.FindAsync(permissionId);
                if (permission == null || role.RolePermissions.Any(rp => rp.Role.Id == roleId && rp.Permission.Id == permissionId))
                    continue;

                role.RolePermissions.Add(new RolePermission
                {
                    Role = role,
                    Permission = permission
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task UnassignPermissionsFromRole(int roleId, List<int> permissionIds)
        {
            var role = await _db.Role.Include(r => r.RolePermissions)
                                     .FirstOrDefaultAsync(r => r.Id == roleId);
            role.ThrowBusinessExceptionIfNull($"Role with id {roleId} does not exist.");
            
            _db.RemoveRange(role.RolePermissions.Where(rp => permissionIds.Contains(rp.PermissionId) && rp.Role.Id == roleId));
            
            await _db.SaveChangesAsync();
        }
    }
}
