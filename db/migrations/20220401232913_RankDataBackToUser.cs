using Microsoft.EntityFrameworkCore.Migrations;

namespace SS.Db.Migrations
{
    public partial class RankDataBackToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"User\" SET \"Rank\" = \"SheriffRank\".\"Rank\" FROM \"SheriffRank\" WHERE \"SheriffRank\".\"SheriffId\" = \"User\".\"Id\"");

            migrationBuilder.DropTable(name: "SheriffRank");
        }
    }
}
