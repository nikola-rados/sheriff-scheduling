using Microsoft.EntityFrameworkCore.Migrations;

namespace SS.Db.Migrations
{
    public partial class AddNoteToSheriffTraining : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SheriffTraining",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "SheriffTraining");
        }
    }
}
