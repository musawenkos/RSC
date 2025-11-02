using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadSignCapture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSignModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Signs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignIdNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SignType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SARTSMCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RouteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NodeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WidthMm = table.Column<double>(type: "float", nullable: false),
                    HeightMm = table.Column<double>(type: "float", nullable: false),
                    AreaM2 = table.Column<double>(type: "float", nullable: false),
                    NumPoles = table.Column<int>(type: "int", nullable: false),
                    PoleDiameter = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PoleLengthMm = table.Column<double>(type: "float", nullable: false),
                    ExcavationDepthMm = table.Column<double>(type: "float", nullable: false),
                    ExcavationCubicM = table.Column<double>(type: "float", nullable: false),
                    SupportType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    OffsetDistanceM = table.Column<double>(type: "float", nullable: false),
                    InstallationStatus = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PdfPath = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    JpgPath = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SgxPath = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    CreatedByUserEmail = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    ProjectName1 = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientEmail = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signs_Project_ProjectName",
                        column: x => x.ProjectName,
                        principalTable: "Project",
                        principalColumn: "ProjectName");
                    table.ForeignKey(
                        name: "FK_Signs_Project_ProjectName1",
                        column: x => x.ProjectName1,
                        principalTable: "Project",
                        principalColumn: "ProjectName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Signs_User_ClientEmail",
                        column: x => x.ClientEmail,
                        principalTable: "User",
                        principalColumn: "Email");
                    table.ForeignKey(
                        name: "FK_Signs_User_CreatedByUserEmail",
                        column: x => x.CreatedByUserEmail,
                        principalTable: "User",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Signs_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "Email");
                    table.ForeignKey(
                        name: "FK_Signs_User_Email",
                        column: x => x.Email,
                        principalTable: "User",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "AuditTrails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    DeviceUsed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocationContext = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditTrails_Signs_SignId",
                        column: x => x.SignId,
                        principalTable: "Signs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    CapturedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Signs_SignId",
                        column: x => x.SignId,
                        principalTable: "Signs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_SignId",
                table: "AuditTrails",
                column: "SignId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_SignId",
                table: "Photos",
                column: "SignId");

            migrationBuilder.CreateIndex(
                name: "IX_Signs_ClientEmail",
                table: "Signs",
                column: "ClientEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Signs_CreatedByUserEmail",
                table: "Signs",
                column: "CreatedByUserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Signs_CreatedByUserId",
                table: "Signs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Signs_Email",
                table: "Signs",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Signs_ProjectName",
                table: "Signs",
                column: "ProjectName");

            migrationBuilder.CreateIndex(
                name: "IX_Signs_ProjectName1",
                table: "Signs",
                column: "ProjectName1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrails");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Signs");
        }
    }
}
