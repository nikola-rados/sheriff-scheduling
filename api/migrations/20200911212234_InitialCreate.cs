using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace SS.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shersched");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "databasechangelog",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 255, nullable: false),
                    author = table.Column<string>(maxLength: 255, nullable: false),
                    filename = table.Column<string>(maxLength: 255, nullable: false),
                    dateexecuted = table.Column<DateTime>(nullable: false),
                    orderexecuted = table.Column<int>(nullable: false),
                    exectype = table.Column<string>(maxLength: 10, nullable: false),
                    md5sum = table.Column<string>(maxLength: 35, nullable: true),
                    description = table.Column<string>(maxLength: 255, nullable: true),
                    comments = table.Column<string>(maxLength: 255, nullable: true),
                    tag = table.Column<string>(maxLength: 255, nullable: true),
                    liquibase = table.Column<string>(maxLength: 20, nullable: true),
                    contexts = table.Column<string>(maxLength: 255, nullable: true),
                    labels = table.Column<string>(maxLength: 255, nullable: true),
                    deployment_id = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "databasechangeloglock",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    locked = table.Column<bool>(nullable: false),
                    lockgranted = table.Column<DateTime>(nullable: true),
                    lockedby = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_databasechangeloglock", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aud_sheriff_duty",
                schema: "shersched",
                columns: table => new
                {
                    sheriff_duty_id = table.Column<Guid>(nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false),
                    operation_code = table.Column<string>(maxLength: 20, nullable: false),
                    operation_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    operation_user_id = table.Column<Guid>(nullable: true),
                    transaction_id = table.Column<Guid>(nullable: true),
                    sheriff_id = table.Column<Guid>(nullable: true),
                    duty_id = table.Column<Guid>(nullable: true),
                    start_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: true),
                    updated_by = table.Column<string>(maxLength: 32, nullable: true),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aud_shrdty", x => new { x.sheriff_duty_id, x.revision_count });
                });

            migrationBuilder.CreateTable(
                name: "auth_api_scope",
                schema: "shersched",
                columns: table => new
                {
                    api_scope_id = table.Column<Guid>(nullable: false),
                    scope_name = table.Column<string>(maxLength: 128, nullable: false),
                    scope_code = table.Column<string>(maxLength: 128, nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    system_scope_ind = table.Column<bool>(nullable: false),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_scope", x => x.api_scope_id);
                });

            migrationBuilder.CreateTable(
                name: "auth_frontend_scope",
                schema: "shersched",
                columns: table => new
                {
                    frontend_scope_id = table.Column<Guid>(nullable: false),
                    scope_name = table.Column<string>(maxLength: 128, nullable: false),
                    scope_code = table.Column<string>(maxLength: 128, nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    system_scope_ind = table.Column<bool>(nullable: false),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_frontend_scope", x => x.frontend_scope_id);
                });

            migrationBuilder.CreateTable(
                name: "auth_role",
                schema: "shersched",
                columns: table => new
                {
                    role_id = table.Column<Guid>(nullable: false),
                    role_name = table.Column<string>(maxLength: 128, nullable: false),
                    role_code = table.Column<string>(maxLength: 128, nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    system_role_ind = table.Column<short>(nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "gender_code",
                schema: "shersched",
                columns: table => new
                {
                    gender_code = table.Column<string>(maxLength: 10, nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gndr", x => x.gender_code);
                },
                comment: "Gender Code captures the standard system values for gender     Male     Female     Other");

            migrationBuilder.CreateTable(
                name: "leave_cancel_reason_code",
                schema: "shersched",
                columns: table => new
                {
                    leave_cancel_reason_code = table.Column<string>(maxLength: 20, nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lvcr", x => x.leave_cancel_reason_code);
                },
                comment: "Leave Cancel Reason Code captures the reasons for cancellation of leave, for BI and auditing purposes. Initial values are     - Operational Demands     - Personal Decision    - Entry Error");

            migrationBuilder.CreateTable(
                name: "leave_code",
                schema: "shersched",
                columns: table => new
                {
                    leave_code = table.Column<string>(maxLength: 20, nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lvcd", x => x.leave_code);
                },
                comment: "Leave Type Code captures the different categories of leave being managed. Initial types are     Leave     Training");

            migrationBuilder.CreateTable(
                name: "region",
                schema: "shersched",
                columns: table => new
                {
                    region_id = table.Column<Guid>(nullable: false),
                    region_cd = table.Column<string>(maxLength: 12, nullable: false, comment: "business key"),
                    region_name = table.Column<string>(maxLength: 100, nullable: false),
                    location = table.Column<NpgsqlPolygon>(nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_region", x => x.region_id);
                });

            migrationBuilder.CreateTable(
                name: "sheriff_rank_code",
                schema: "shersched",
                columns: table => new
                {
                    sheriff_rank_code = table.Column<string>(maxLength: 20, nullable: false),
                    rank_order = table.Column<int>(nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shrkcd", x => x.sheriff_rank_code);
                });

            migrationBuilder.CreateTable(
                name: "work_section_code",
                schema: "shersched",
                columns: table => new
                {
                    work_section_code = table.Column<string>(maxLength: 20, nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wksccd", x => x.work_section_code);
                });

            migrationBuilder.CreateTable(
                name: "auth_frontend_scope_api",
                schema: "shersched",
                columns: table => new
                {
                    frontend_scope_api_id = table.Column<Guid>(nullable: false),
                    frontend_scope_id = table.Column<Guid>(nullable: false),
                    api_scope_id = table.Column<Guid>(nullable: false),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_frontend_scope_api", x => x.frontend_scope_api_id);
                    table.ForeignKey(
                        name: "fk_api_scope_id",
                        column: x => x.api_scope_id,
                        principalSchema: "shersched",
                        principalTable: "auth_api_scope",
                        principalColumn: "api_scope_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_frontend_scope_id",
                        column: x => x.frontend_scope_id,
                        principalSchema: "shersched",
                        principalTable: "auth_frontend_scope",
                        principalColumn: "frontend_scope_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auth_frontend_scope_permission",
                schema: "shersched",
                columns: table => new
                {
                    frontend_scope_permission_id = table.Column<Guid>(nullable: false),
                    frontend_scope_id = table.Column<Guid>(nullable: false),
                    display_name = table.Column<string>(maxLength: 128, nullable: false),
                    permission_code = table.Column<string>(maxLength: 128, nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_frontend_scope_permission", x => x.frontend_scope_permission_id);
                    table.ForeignKey(
                        name: "fk_frontend_scope",
                        column: x => x.frontend_scope_id,
                        principalSchema: "shersched",
                        principalTable: "auth_frontend_scope",
                        principalColumn: "frontend_scope_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auth_role_api_scope",
                schema: "shersched",
                columns: table => new
                {
                    role_api_scope_id = table.Column<Guid>(nullable: false),
                    role_id = table.Column<Guid>(nullable: false),
                    api_scope_id = table.Column<Guid>(nullable: false),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_api_scope", x => x.role_api_scope_id);
                    table.ForeignKey(
                        name: "fk_api_scope_id",
                        column: x => x.api_scope_id,
                        principalSchema: "shersched",
                        principalTable: "auth_api_scope",
                        principalColumn: "api_scope_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_role_id",
                        column: x => x.role_id,
                        principalSchema: "shersched",
                        principalTable: "auth_role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auth_role_frontend_scope",
                schema: "shersched",
                columns: table => new
                {
                    role_frontend_scope_id = table.Column<Guid>(nullable: false),
                    role_id = table.Column<Guid>(nullable: false),
                    frontend_scope_id = table.Column<Guid>(nullable: false),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_frontend_scope", x => x.role_frontend_scope_id);
                    table.ForeignKey(
                        name: "fk_frontend_scope_id",
                        column: x => x.frontend_scope_id,
                        principalSchema: "shersched",
                        principalTable: "auth_frontend_scope",
                        principalColumn: "frontend_scope_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_role_id",
                        column: x => x.role_id,
                        principalSchema: "shersched",
                        principalTable: "auth_role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "leave_sub_code",
                schema: "shersched",
                columns: table => new
                {
                    leave_code = table.Column<string>(maxLength: 20, nullable: false),
                    leave_sub_code = table.Column<string>(maxLength: 20, nullable: false),
                    description = table.Column<string>(maxLength: 200, nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false),
                    sort_order = table.Column<decimal>(type: "numeric(3,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lvsbcd", x => new { x.leave_code, x.leave_sub_code });
                    table.ForeignKey(
                        name: "fk_lvsbcd_lvcd",
                        column: x => x.leave_code,
                        principalSchema: "shersched",
                        principalTable: "leave_code",
                        principalColumn: "leave_code",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Leave Type Sub Code captures the different types of leave being managed in each category. Initial types are     Leave -     STIP (Short Term Illness)     Leave -     Annual Leave     Leave -     Special Leave     Training -  Training");

            migrationBuilder.CreateTable(
                name: "location",
                schema: "shersched",
                columns: table => new
                {
                    location_id = table.Column<Guid>(nullable: false),
                    location_cd = table.Column<string>(maxLength: 5, nullable: false, comment: "business key"),
                    justin_id = table.Column<string>(maxLength: 20, nullable: true),
                    justin_code = table.Column<string>(maxLength: 10, nullable: true),
                    location_name = table.Column<string>(maxLength: 100, nullable: false),
                    parent_location_id = table.Column<Guid>(nullable: true),
                    region_id = table.Column<Guid>(nullable: false),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location", x => x.location_id);
                    table.ForeignKey(
                        name: "fk_locn_reg",
                        column: x => x.region_id,
                        principalSchema: "shersched",
                        principalTable: "region",
                        principalColumn: "region_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auth_role_permission",
                schema: "shersched",
                columns: table => new
                {
                    role_permission_id = table.Column<Guid>(nullable: false),
                    role_id = table.Column<Guid>(nullable: true),
                    role_frontend_scope_id = table.Column<Guid>(nullable: true),
                    role_api_scope_id = table.Column<Guid>(nullable: true),
                    frontend_scope_permission_id = table.Column<Guid>(nullable: true),
                    api_scope_permission_id = table.Column<Guid>(nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permission", x => x.role_permission_id);
                    table.ForeignKey(
                        name: "fk_frontend_permission_id",
                        column: x => x.frontend_scope_permission_id,
                        principalSchema: "shersched",
                        principalTable: "auth_frontend_scope_permission",
                        principalColumn: "frontend_scope_permission_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_role_api_scope_id",
                        column: x => x.role_api_scope_id,
                        principalSchema: "shersched",
                        principalTable: "auth_role_api_scope",
                        principalColumn: "role_api_scope_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_role_frontend_scope_id",
                        column: x => x.role_frontend_scope_id,
                        principalSchema: "shersched",
                        principalTable: "auth_role_frontend_scope",
                        principalColumn: "role_frontend_scope_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_role_id",
                        column: x => x.role_id,
                        principalSchema: "shersched",
                        principalTable: "auth_role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auth_user_role",
                schema: "shersched",
                columns: table => new
                {
                    user_role_id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    role_id = table.Column<Guid>(nullable: false),
                    location_id = table.Column<Guid>(nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_role", x => x.user_role_id);
                    table.ForeignKey(
                        name: "fk_location_id",
                        column: x => x.location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_role_id",
                        column: x => x.role_id,
                        principalSchema: "shersched",
                        principalTable: "auth_role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "court_role_code",
                schema: "shersched",
                columns: table => new
                {
                    court_role_id = table.Column<Guid>(nullable: false),
                    location_id = table.Column<Guid>(nullable: true),
                    court_role_code = table.Column<string>(maxLength: 64, nullable: true),
                    court_role_name = table.Column<string>(maxLength: 128, nullable: true),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    sort_order = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_court_role_code", x => x.court_role_id);
                    table.ForeignKey(
                        name: "fk_location_id",
                        column: x => x.location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "courtroom",
                schema: "shersched",
                columns: table => new
                {
                    courtroom_id = table.Column<Guid>(nullable: false),
                    location_id = table.Column<Guid>(nullable: true),
                    courtroom_code = table.Column<string>(maxLength: 64, nullable: true),
                    courtroom_name = table.Column<string>(maxLength: 128, nullable: true),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    sort_order = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courtroom", x => x.courtroom_id);
                    table.ForeignKey(
                        name: "fk_location_id",
                        column: x => x.location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "escort_run",
                schema: "shersched",
                columns: table => new
                {
                    escort_run_id = table.Column<Guid>(nullable: false),
                    location_id = table.Column<Guid>(nullable: true),
                    escort_run_code = table.Column<string>(maxLength: 64, nullable: true),
                    escort_run_name = table.Column<string>(maxLength: 128, nullable: true),
                    title = table.Column<string>(maxLength: 128, nullable: true),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    sort_order = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escort_run", x => x.escort_run_id);
                    table.ForeignKey(
                        name: "fk_location_id",
                        column: x => x.location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "jail_role_code",
                schema: "shersched",
                columns: table => new
                {
                    jail_role_id = table.Column<Guid>(nullable: false),
                    location_id = table.Column<Guid>(nullable: true),
                    jail_role_code = table.Column<string>(maxLength: 64, nullable: true),
                    jail_role_name = table.Column<string>(maxLength: 128, nullable: true),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    sort_order = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jail_role_code", x => x.jail_role_id);
                    table.ForeignKey(
                        name: "fk_location_id",
                        column: x => x.location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "other_assign_code",
                schema: "shersched",
                columns: table => new
                {
                    other_assign_id = table.Column<Guid>(nullable: false),
                    location_id = table.Column<Guid>(nullable: true),
                    other_assign_code = table.Column<string>(maxLength: 64, nullable: true),
                    other_assign_name = table.Column<string>(maxLength: 128, nullable: true),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    sort_order = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_other_assign_code", x => x.other_assign_id);
                    table.ForeignKey(
                        name: "fk_location_id",
                        column: x => x.location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sheriff",
                schema: "shersched",
                columns: table => new
                {
                    sheriff_id = table.Column<Guid>(nullable: false),
                    badge_no = table.Column<string>(maxLength: 20, nullable: false),
                    first_name = table.Column<string>(maxLength: 100, nullable: true),
                    last_name = table.Column<string>(maxLength: 100, nullable: true),
                    image_url = table.Column<string>(maxLength: 200, nullable: true, comment: "TBD how to store a Sheriff photo"),
                    sheriff_rank_code = table.Column<string>(maxLength: 20, nullable: false),
                    home_location_id = table.Column<Guid>(nullable: false, comment: "Permanent location for a sheriff"),
                    current_location_id = table.Column<Guid>(nullable: true),
                    alias = table.Column<string>(maxLength: 32, nullable: true),
                    gender_code = table.Column<string>(maxLength: 10, nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheriff", x => x.sheriff_id);
                    table.ForeignKey(
                        name: "fk_shr_crlocn",
                        column: x => x.current_location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shrf_gndr",
                        column: x => x.gender_code,
                        principalSchema: "shersched",
                        principalTable: "gender_code",
                        principalColumn: "gender_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shr_hmlocn",
                        column: x => x.home_location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shr_shrkcd",
                        column: x => x.sheriff_rank_code,
                        principalSchema: "shersched",
                        principalTable: "sheriff_rank_code",
                        principalColumn: "sheriff_rank_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "assignment",
                schema: "shersched",
                columns: table => new
                {
                    assignment_id = table.Column<Guid>(nullable: false),
                    title = table.Column<string>(maxLength: 100, nullable: true),
                    work_section_code = table.Column<string>(maxLength: 20, nullable: false),
                    location_id = table.Column<Guid>(nullable: false),
                    courtroom_id = table.Column<Guid>(nullable: true),
                    court_role_id = table.Column<Guid>(nullable: true),
                    jail_role_id = table.Column<Guid>(nullable: true),
                    escort_run_id = table.Column<Guid>(nullable: true),
                    other_assign_id = table.Column<Guid>(nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    sort_order = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignment", x => x.assignment_id);
                    table.ForeignKey(
                        name: "fk_court_role_id",
                        column: x => x.court_role_id,
                        principalSchema: "shersched",
                        principalTable: "court_role_code",
                        principalColumn: "court_role_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_courtroom_id",
                        column: x => x.courtroom_id,
                        principalSchema: "shersched",
                        principalTable: "courtroom",
                        principalColumn: "courtroom_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_escort_run_id",
                        column: x => x.escort_run_id,
                        principalSchema: "shersched",
                        principalTable: "escort_run",
                        principalColumn: "escort_run_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_jail_role_id",
                        column: x => x.jail_role_id,
                        principalSchema: "shersched",
                        principalTable: "jail_role_code",
                        principalColumn: "jail_role_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_location_id",
                        column: x => x.location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_other_assign_id",
                        column: x => x.other_assign_id,
                        principalSchema: "shersched",
                        principalTable: "other_assign_code",
                        principalColumn: "other_assign_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auth_user",
                schema: "shersched",
                columns: table => new
                {
                    user_id = table.Column<Guid>(nullable: false),
                    user_auth_id = table.Column<string>(maxLength: 100, nullable: true),
                    siteminder_id = table.Column<string>(maxLength: 100, nullable: true),
                    display_name = table.Column<string>(maxLength: 100, nullable: false),
                    sheriff_id = table.Column<Guid>(nullable: true),
                    default_location_id = table.Column<Guid>(nullable: true),
                    system_account_ind = table.Column<int>(nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_sheriff_id",
                        column: x => x.sheriff_id,
                        principalSchema: "shersched",
                        principalTable: "sheriff",
                        principalColumn: "sheriff_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "leave",
                schema: "shersched",
                columns: table => new
                {
                    leave_id = table.Column<Guid>(nullable: false),
                    sheriff_id = table.Column<Guid>(nullable: false),
                    leave_code = table.Column<string>(maxLength: 20, nullable: false),
                    leave_sub_code = table.Column<string>(maxLength: 20, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    start_time = table.Column<DateTimeOffset>(type: "time with time zone", nullable: true),
                    end_time = table.Column<DateTimeOffset>(type: "time with time zone", nullable: true),
                    partial_day_ind = table.Column<int>(nullable: false),
                    comment = table.Column<string>(maxLength: 200, nullable: true),
                    cancelled_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    leave_cancel_reason_code = table.Column<string>(maxLength: 20, nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leave", x => x.leave_id);
                    table.ForeignKey(
                        name: "fk_lve_lvcr",
                        column: x => x.leave_cancel_reason_code,
                        principalSchema: "shersched",
                        principalTable: "leave_cancel_reason_code",
                        principalColumn: "leave_cancel_reason_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_lve_shrf",
                        column: x => x.sheriff_id,
                        principalSchema: "shersched",
                        principalTable: "sheriff",
                        principalColumn: "sheriff_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_lve_lvsbcd",
                        columns: x => new { x.leave_code, x.leave_sub_code },
                        principalSchema: "shersched",
                        principalTable: "leave_sub_code",
                        principalColumns: new[] { "leave_code", "leave_sub_code" },
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Leave captures data related to absence from regular duty for one or more full days.");

            migrationBuilder.CreateTable(
                name: "sheriff_location",
                schema: "shersched",
                columns: table => new
                {
                    sheriff_location_id = table.Column<Guid>(nullable: false),
                    sheriff_id = table.Column<Guid>(nullable: true),
                    location_id = table.Column<Guid>(nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    start_time = table.Column<TimeSpan>(type: "time without time zone", nullable: true),
                    end_time = table.Column<TimeSpan>(type: "time without time zone", nullable: true),
                    partial_day_ind = table.Column<int>(nullable: false),
                    sort_order = table.Column<decimal>(type: "numeric(3,0)", nullable: true),
                    created_by = table.Column<string>(maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(maxLength: 100, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheriff_location", x => x.sheriff_location_id);
                    table.ForeignKey(
                        name: "fk_location_id",
                        column: x => x.location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sheriff_id",
                        column: x => x.sheriff_id,
                        principalSchema: "shersched",
                        principalTable: "sheriff",
                        principalColumn: "sheriff_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "duty_recurrence",
                schema: "shersched",
                columns: table => new
                {
                    duty_recurrence_id = table.Column<Guid>(nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "time with time zone", nullable: true),
                    end_time = table.Column<DateTimeOffset>(type: "time with time zone", nullable: true),
                    days_bitmap = table.Column<decimal>(type: "numeric(10,0)", nullable: false, comment: "1=mo,2=tu,4=we,8=th,16=fr,32=sa,64=su"),
                    sheriffs_required = table.Column<decimal>(type: "numeric(2,0)", nullable: false),
                    assignment_id = table.Column<Guid>(nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "now()"),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_duty_recurrence", x => x.duty_recurrence_id);
                    table.ForeignKey(
                        name: "fk_dtyrc_asn",
                        column: x => x.assignment_id,
                        principalSchema: "shersched",
                        principalTable: "assignment",
                        principalColumn: "assignment_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shift",
                schema: "shersched",
                columns: table => new
                {
                    shift_id = table.Column<Guid>(nullable: false),
                    work_section_code = table.Column<string>(maxLength: 20, nullable: true),
                    assignment_id = table.Column<Guid>(nullable: true),
                    location_id = table.Column<Guid>(nullable: false),
                    sheriff_id = table.Column<Guid>(nullable: true),
                    start_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shift", x => x.shift_id);
                    table.ForeignKey(
                        name: "fk_shft_asn",
                        column: x => x.assignment_id,
                        principalSchema: "shersched",
                        principalTable: "assignment",
                        principalColumn: "assignment_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shft_locn",
                        column: x => x.location_id,
                        principalSchema: "shersched",
                        principalTable: "location",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shft_shr",
                        column: x => x.sheriff_id,
                        principalSchema: "shersched",
                        principalTable: "sheriff",
                        principalColumn: "sheriff_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shft_wsc",
                        column: x => x.work_section_code,
                        principalSchema: "shersched",
                        principalTable: "work_section_code",
                        principalColumn: "work_section_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "duty",
                schema: "shersched",
                columns: table => new
                {
                    duty_id = table.Column<Guid>(nullable: false),
                    duty_recurrence_id = table.Column<Guid>(nullable: true),
                    start_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    assignment_id = table.Column<Guid>(nullable: false),
                    comment = table.Column<string>(maxLength: 200, nullable: true),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_duty", x => x.duty_id);
                    table.ForeignKey(
                        name: "fk_dty_asn",
                        column: x => x.assignment_id,
                        principalSchema: "shersched",
                        principalTable: "assignment",
                        principalColumn: "assignment_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_dty_dtyrc",
                        column: x => x.duty_recurrence_id,
                        principalSchema: "shersched",
                        principalTable: "duty_recurrence",
                        principalColumn: "duty_recurrence_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sheriff_duty",
                schema: "shersched",
                columns: table => new
                {
                    sheriff_duty_id = table.Column<Guid>(nullable: false),
                    sheriff_id = table.Column<Guid>(nullable: true),
                    duty_id = table.Column<Guid>(nullable: false),
                    start_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(maxLength: 32, nullable: false),
                    updated_by = table.Column<string>(maxLength: 32, nullable: false),
                    created_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_dtm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revision_count = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheriff_duty", x => x.sheriff_duty_id);
                    table.ForeignKey(
                        name: "fk_shrdty_dty",
                        column: x => x.duty_id,
                        principalSchema: "shersched",
                        principalTable: "duty",
                        principalColumn: "duty_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shrdty_shr",
                        column: x => x.sheriff_id,
                        principalSchema: "shersched",
                        principalTable: "sheriff",
                        principalColumn: "sheriff_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_assignment_court_role",
                schema: "shersched",
                table: "assignment",
                column: "court_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_courtroom",
                schema: "shersched",
                table: "assignment",
                column: "courtroom_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_escort_run",
                schema: "shersched",
                table: "assignment",
                column: "escort_run_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_jail_role",
                schema: "shersched",
                table: "assignment",
                column: "jail_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_location",
                schema: "shersched",
                table: "assignment",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_other_assign_role",
                schema: "shersched",
                table: "assignment",
                column: "other_assign_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_work_section_code",
                schema: "shersched",
                table: "assignment",
                column: "work_section_code");

            migrationBuilder.CreateIndex(
                name: "IX_auth_frontend_scope_api_api_scope_id",
                schema: "shersched",
                table: "auth_frontend_scope_api",
                column: "api_scope_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_frontend_scope_api_frontend_scope_id",
                schema: "shersched",
                table: "auth_frontend_scope_api",
                column: "frontend_scope_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_frontend_scope_permission_frontend_scope_id",
                schema: "shersched",
                table: "auth_frontend_scope_permission",
                column: "frontend_scope_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_role_api_scope_api_scope_id",
                schema: "shersched",
                table: "auth_role_api_scope",
                column: "api_scope_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_role_api_scope_role_id",
                schema: "shersched",
                table: "auth_role_api_scope",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_role_frontend_scope_frontend_scope_id",
                schema: "shersched",
                table: "auth_role_frontend_scope",
                column: "frontend_scope_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_role_frontend_scope_role_id",
                schema: "shersched",
                table: "auth_role_frontend_scope",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_role_permission_frontend_scope_permission_id",
                schema: "shersched",
                table: "auth_role_permission",
                column: "frontend_scope_permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_role_permission_role_api_scope_id",
                schema: "shersched",
                table: "auth_role_permission",
                column: "role_api_scope_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_role_permission_role_frontend_scope_id",
                schema: "shersched",
                table: "auth_role_permission",
                column: "role_frontend_scope_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_role_permission_role_id",
                schema: "shersched",
                table: "auth_role_permission",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_user_sheriff_id",
                schema: "shersched",
                table: "auth_user",
                column: "sheriff_id");

            migrationBuilder.CreateIndex(
                name: "ix_location",
                schema: "shersched",
                table: "auth_user_role",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_role",
                schema: "shersched",
                table: "auth_user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user",
                schema: "shersched",
                table: "auth_user_role",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "court_role_code_loc_isnull_idx",
                schema: "shersched",
                table: "court_role_code",
                column: "court_role_code",
                unique: true,
                filter: "(location_id IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_court_role_code_location_id",
                schema: "shersched",
                table: "court_role_code",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "court_role_code_loc_notnull_idx",
                schema: "shersched",
                table: "court_role_code",
                columns: new[] { "court_role_code", "location_id" },
                unique: true,
                filter: "(location_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "courtroom_loc_isnull_idx",
                schema: "shersched",
                table: "courtroom",
                column: "courtroom_code",
                unique: true,
                filter: "(location_id IS NULL)");

            migrationBuilder.CreateIndex(
                name: "ix_courtroom_name",
                schema: "shersched",
                table: "courtroom",
                column: "courtroom_name");

            migrationBuilder.CreateIndex(
                name: "IX_courtroom_location_id",
                schema: "shersched",
                table: "courtroom",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "courtroom_loc_notnull_idx",
                schema: "shersched",
                table: "courtroom",
                columns: new[] { "courtroom_code", "location_id" },
                unique: true,
                filter: "(location_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ix_dty_asn",
                schema: "shersched",
                table: "duty",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "ix_dty_dtyrc",
                schema: "shersched",
                table: "duty",
                column: "duty_recurrence_id");

            migrationBuilder.CreateIndex(
                name: "ix_dtyrc_asn",
                schema: "shersched",
                table: "duty_recurrence",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "escort_run_loc_isnull_idx",
                schema: "shersched",
                table: "escort_run",
                column: "escort_run_code",
                unique: true,
                filter: "(location_id IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_escort_run_location_id",
                schema: "shersched",
                table: "escort_run",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "escort_run_loc_notnull_idx",
                schema: "shersched",
                table: "escort_run",
                columns: new[] { "escort_run_code", "location_id" },
                unique: true,
                filter: "(location_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "jail_role_code_loc_isnull_idx",
                schema: "shersched",
                table: "jail_role_code",
                column: "jail_role_code",
                unique: true,
                filter: "(location_id IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_jail_role_code_location_id",
                schema: "shersched",
                table: "jail_role_code",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "jail_role_code_loc_notnull_idx",
                schema: "shersched",
                table: "jail_role_code",
                columns: new[] { "jail_role_code", "location_id" },
                unique: true,
                filter: "(location_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_leave_leave_cancel_reason_code",
                schema: "shersched",
                table: "leave",
                column: "leave_cancel_reason_code");

            migrationBuilder.CreateIndex(
                name: "IX_leave_sheriff_id",
                schema: "shersched",
                table: "leave",
                column: "sheriff_id");

            migrationBuilder.CreateIndex(
                name: "IX_leave_leave_code_leave_sub_code",
                schema: "shersched",
                table: "leave",
                columns: new[] { "leave_code", "leave_sub_code" });

            migrationBuilder.CreateIndex(
                name: "uk_locn_cd",
                schema: "shersched",
                table: "location",
                column: "location_cd",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_locn_prt",
                schema: "shersched",
                table: "location",
                column: "parent_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_region_id",
                schema: "shersched",
                table: "location",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "IX_other_assign_code_location_id",
                schema: "shersched",
                table: "other_assign_code",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "other_assign_code_loc_isnull_idx",
                schema: "shersched",
                table: "other_assign_code",
                column: "other_assign_code",
                unique: true,
                filter: "(location_id IS NULL)");

            migrationBuilder.CreateIndex(
                name: "other_assign_code_loc_notnull_idx",
                schema: "shersched",
                table: "other_assign_code",
                columns: new[] { "other_assign_code", "location_id" },
                unique: true,
                filter: "(location_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "uk_rgn_cd",
                schema: "shersched",
                table: "region",
                column: "region_cd",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_shrf_bdgn",
                schema: "shersched",
                table: "sheriff",
                column: "badge_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shr_crlocn",
                schema: "shersched",
                table: "sheriff",
                column: "current_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_sheriff_gender_code",
                schema: "shersched",
                table: "sheriff",
                column: "gender_code");

            migrationBuilder.CreateIndex(
                name: "ix_shr_hmlocn",
                schema: "shersched",
                table: "sheriff",
                column: "home_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_shr_rkcd",
                schema: "shersched",
                table: "sheriff",
                column: "sheriff_rank_code");

            migrationBuilder.CreateIndex(
                name: "ix_shrdty_dty",
                schema: "shersched",
                table: "sheriff_duty",
                column: "duty_id");

            migrationBuilder.CreateIndex(
                name: "ix_shrdty_shr",
                schema: "shersched",
                table: "sheriff_duty",
                column: "sheriff_id");

            migrationBuilder.CreateIndex(
                name: "IX_sheriff_location_location_id",
                schema: "shersched",
                table: "sheriff_location",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_sheriff_location_sheriff_id",
                schema: "shersched",
                table: "sheriff_location",
                column: "sheriff_id");

            migrationBuilder.CreateIndex(
                name: "IX_shift_assignment_id",
                schema: "shersched",
                table: "shift",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "ix_shft_locn",
                schema: "shersched",
                table: "shift",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_shft_shr",
                schema: "shersched",
                table: "shift",
                column: "sheriff_id");

            migrationBuilder.CreateIndex(
                name: "ix_shft_wsc",
                schema: "shersched",
                table: "shift",
                column: "work_section_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "databasechangelog");

            migrationBuilder.DropTable(
                name: "databasechangeloglock");

            migrationBuilder.DropTable(
                name: "aud_sheriff_duty",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_frontend_scope_api",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_role_permission",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_user",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_user_role",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "leave",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "sheriff_duty",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "sheriff_location",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "shift",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_frontend_scope_permission",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_role_api_scope",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_role_frontend_scope",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "leave_cancel_reason_code",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "leave_sub_code",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "duty",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "sheriff",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "work_section_code",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_api_scope",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_frontend_scope",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "auth_role",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "leave_code",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "duty_recurrence",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "gender_code",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "sheriff_rank_code",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "assignment",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "court_role_code",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "courtroom",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "escort_run",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "jail_role_code",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "other_assign_code",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "location",
                schema: "shersched");

            migrationBuilder.DropTable(
                name: "region",
                schema: "shersched");
        }
    }
}
