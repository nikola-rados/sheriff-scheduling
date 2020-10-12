DELETE FROM "RolePermission";

SET TIMEZONE='UTC';
\set SystemUserId '00000000-0000-0000-0000-000000000001';
\set AdministratorId 'SELECT "Id" FROM "Role" WHERE "Name" = 'Administrator'';
\set ManagerId 'SELECT "Id" FROM "Role" WHERE "Name" = 'Manager''; 
\set SheriffId 'SELECT "Id" FROM "Role" WHERE "Name" = 'Sheriff''; 

-- Administrator
INSERT INTO "RolePermission" ("CreatedById","CreatedOn","UpdatedById","UpdatedOn","RoleId","PermissionId") VALUES 
(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'Login')),
(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewRegion'));

-- Manager
INSERT INTO "RolePermission" ("CreatedById","CreatedOn","UpdatedById","UpdatedOn","RoleId","PermissionId") VALUES 
(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'Login')),
(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewRegion'));

-- Sheriff
INSERT INTO "RolePermission" ("CreatedById","CreatedOn","UpdatedById","UpdatedOn","RoleId","PermissionId") VALUES 
(SystemUserId,now(),NULL,NULL, SheriffId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'Login')),
(SystemUserId,now(),NULL,NULL, SheriffId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewRegion'));
