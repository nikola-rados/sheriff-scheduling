using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
            return await _db.Role.AsNoTracking().ToListAsync();
        }

        public async Task<Role> Role(int id)
        {
            return await _db.Role.AsNoTracking().AsSingleQuery().Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission).SingleOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role> AddRole(Role role, List<int> permissionIds)
        {
            var roleAlreadyExistsWithName = await _db.Role.AnyAsync(r => r.Name.ToLower() == role.Name.ToLower());
            if (roleAlreadyExistsWithName)
                throw new BusinessLayerException($"Role with name {role.Name} already exists.");

            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            role.RolePermissions = null;
            role.UserRoles = null;
            await _db.Role.AddAsync(role);
            await _db.SaveChangesAsync();
            await AssignPermissionsToRole(role.Id, permissionIds);
            await _db.SaveChangesAsync();
            scope.Complete();
            return role;
        }

        public async Task RemoveRole(int roleId)
        {
            var role = await _db.Role.FindAsync(roleId);
            _db.Role.Remove(role);
            await _db.SaveChangesAsync();
        }

        public async Task<Role> UpdateRole(Role role, List<int> permissionIds)
        {
            var savedRole = await _db.Role.AsSingleQuery().Include(r => r.RolePermissions)
                                          .FirstOrDefaultAsync(r => r.Id == role.Id);
            if (savedRole.Name != role.Name)
            {
                var roleAlreadyExistsWithName = await _db.Role.AnyAsync(r => r.Name.ToLower() == role.Name.ToLower());
                if (roleAlreadyExistsWithName)
                    throw new BusinessLayerException($"Role with name {role.Name} already exists.");
            }

            var permissionIdsToRemove =
                savedRole.RolePermissions.Select(rp => rp.PermissionId).Except(permissionIds).ToList();

            _db.Entry(savedRole).CurrentValues.SetValues(role);

            await AssignPermissionsToRole(role.Id, permissionIds);
            await UnassignPermissionsFromRole(role.Id, permissionIdsToRemove);
            await _db.SaveChangesAsync();

            return savedRole;
        }

        private async Task AssignPermissionsToRole(int roleId, List<int> permissionIds)
        {
            var role = await _db.Role.AsSingleQuery().Include(r => r.RolePermissions)
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
        }

        private async Task UnassignPermissionsFromRole(int roleId, List<int> permissionIds)
        {
            var role = await _db.Role.AsSingleQuery().Include(r => r.RolePermissions)
                                     .FirstOrDefaultAsync(r => r.Id == roleId);
            role.ThrowBusinessExceptionIfNull($"Role with id {roleId} does not exist.");
            
            _db.RemoveRange(role.RolePermissions.Where(rp => permissionIds.Contains(rp.PermissionId) && rp.Role.Id == roleId));
        }
    }
}
