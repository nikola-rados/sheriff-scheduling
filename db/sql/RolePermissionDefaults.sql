do $$
declare
	SystemUserId uuid; 
	AdministratorId integer;
	ManagerId integer;
	SheriffId integer;
BEGIN
	SystemUserId := '00000000-0000-0000-0000-000000000001';

	DELETE FROM "RolePermission";
	SELECT "Id" INTO AdministratorId FROM "Role" WHERE "Name" = 'Administrator';
	SELECT "Id" INTO ManagerId FROM "Role" WHERE "Name" = 'Manager';
	SELECT "Id" INTO SheriffId FROM "Role" WHERE "Name" = 'Sheriff';

	-- Sheriff
	INSERT INTO "RolePermission" ("CreatedById","CreatedOn","UpdatedById","UpdatedOn","RoleId","PermissionId") VALUES 
	(SystemUserId,now(),NULL,NULL, SheriffId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'Login')),
	(SystemUserId,now(),NULL,NULL, SheriffId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewOwnProfile')),
	(SystemUserId,now(),NULL,NULL, SheriffId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewMyShifts')),
	(SystemUserId,now(),NULL,NULL, SheriffId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewDistributeSchedule')),
	(SystemUserId,now(),NULL,NULL, SheriffId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewHomeLocationAndAssignedLocation'));

	-- Manager
	INSERT INTO "RolePermission" ("CreatedById","CreatedOn","UpdatedById","UpdatedOn","RoleId","PermissionId") VALUES 
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'Login')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewOwnProfile')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewProfilesInOwnLocation')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditUsers')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewMyShifts')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewAllShiftsAtMyLocation')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateAndAssignShifts')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireShifts')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditShifts')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewDistributeSchedule')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewHomeLocationAndAssignedLocation')),
	(SystemUserId,now(),NULL,NULL, ManagerId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewRegion'));

	-- Administrator
	INSERT INTO "RolePermission" ("CreatedById","CreatedOn","UpdatedById","UpdatedOn","RoleId","PermissionId") VALUES 
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'Login')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewOwnProfile')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewProfilesInOwnLocation')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewProfilesInAllLocation')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateUsers')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireUsers')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditUsers')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewRoles')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateAndAssignRoles')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireRoles')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditRoles')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewManageTypes')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateTypes')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditTypes')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireTypes')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewMyShifts')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewAllShiftsAtMyLocation')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'CreateAndAssignShifts')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireShifts')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'EditShifts')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewDistributeSchedule')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewHomeLocationAndAssignedLocation')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewRegion')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ViewProvince')),
	(SystemUserId,now(),NULL,NULL,AdministratorId, (SELECT "Id" FROM "Permission" WHERE "Name" = 'ExpireLocation'));
end;
$$;