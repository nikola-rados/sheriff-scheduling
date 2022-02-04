using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SS.Db.Migrations
{
    public partial class SS681Rank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SheriffRank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SheriffId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rank = table.Column<string>(type: "text", nullable: true),
                    EffectiveDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheriffRank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SheriffRank_User_SheriffId",
                        column: x => x.SheriffId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SheriffRank_SheriffId",
                table: "SheriffRank",
                column: "SheriffId");

            migrationBuilder.Sql("INSERT INTO \"SheriffRank\" (\"SheriffId\", \"Rank\", \"EffectiveDate\") SELECT \"User\".\"Id\" as \"SheriffId\", \"User\".\"Rank\", '2000-01-01' from \"User\"");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SheriffRank");

            migrationBuilder.AddColumn<string>(
                name: "Rank",
                table: "User",
                type: "text",
                nullable: true);
        }
    }
}
