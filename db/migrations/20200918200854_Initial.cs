using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SS.Db.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    BadgeNumber = table.Column<string>(nullable: true),
                    Rank = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedById1 = table.Column<Guid>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LastLogin = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthUser_AuthUser_UpdatedById1",
                        column: x => x.UpdatedById1,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthPermission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    AuthUserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthPermission_AuthUser_AuthUserId",
                        column: x => x.AuthUserId,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    AuthUserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthRole_AuthUser_AuthUserId",
                        column: x => x.AuthUserId,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedById = table.Column<Guid>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedById = table.Column<Guid>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    JustinId = table.Column<int>(nullable: false),
                    JustinCode = table.Column<string>(nullable: true),
                    ParentLocationId = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_AuthUser_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Location_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Location_AuthUser_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LookupCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedById = table.Column<Guid>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedById = table.Column<Guid>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    SubCode = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    SortOrder = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LookupCode_AuthUser_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LookupCode_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LookupCode_AuthUser_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthPermission_AuthUserId",
                table: "AuthPermission",
                column: "AuthUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRole_AuthUserId",
                table: "AuthRole",
                column: "AuthUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_UpdatedById1",
                table: "AuthUser",
                column: "UpdatedById1");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CreatedById",
                table: "Location",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Location_RegionId",
                table: "Location",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_UpdatedById",
                table: "Location",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LookupCode_CreatedById",
                table: "LookupCode",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LookupCode_LocationId",
                table: "LookupCode",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupCode_UpdatedById",
                table: "LookupCode",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthPermission");

            migrationBuilder.DropTable(
                name: "AuthRole");

            migrationBuilder.DropTable(
                name: "LookupCode");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "AuthUser");

            migrationBuilder.DropTable(
                name: "Region");
        }
    }
}
