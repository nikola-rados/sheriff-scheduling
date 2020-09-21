Unused:
EF Scaffold command (reads from secrets):
Scaffold-DbContext "Name=ConnectionStrings.DB" Npgsql.EntityFrameworkCore.PostgreSQL -o Models\DB -DataAnnotations -Context SheriffContext

Add-Migration Migration
Update-Database
Remove-Migration