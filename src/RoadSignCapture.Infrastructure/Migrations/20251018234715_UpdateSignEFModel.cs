using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadSignCapture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSignEFModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Signs_Project_ProjectName",
                table: "Signs");

            migrationBuilder.DropForeignKey(
                name: "FK_Signs_Project_ProjectName1",
                table: "Signs");

            migrationBuilder.DropForeignKey(
                name: "FK_Signs_User_ClientEmail",
                table: "Signs");

            migrationBuilder.DropForeignKey(
                name: "FK_Signs_User_CreatedByUserEmail",
                table: "Signs");

            migrationBuilder.DropForeignKey(
                name: "FK_Signs_User_Email",
                table: "Signs");

            migrationBuilder.DropIndex(
                name: "IX_Signs_ClientEmail",
                table: "Signs");

            migrationBuilder.DropIndex(
                name: "IX_Signs_CreatedByUserEmail",
                table: "Signs");

            migrationBuilder.DropIndex(
                name: "IX_Signs_ProjectName1",
                table: "Signs");

            migrationBuilder.DropColumn(
                name: "ClientEmail",
                table: "Signs");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "Signs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserEmail",
                table: "Signs");

            migrationBuilder.DropColumn(
                name: "ProjectName1",
                table: "Signs");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "Signs",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Signs",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Signs_ProjectName",
                table: "Signs",
                newName: "IX_Signs_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Signs_Email",
                table: "Signs",
                newName: "IX_Signs_ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Signs_Project_ProjectId",
                table: "Signs",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "ProjectName");

            migrationBuilder.AddForeignKey(
                name: "FK_Signs_User_ClientId",
                table: "Signs",
                column: "ClientId",
                principalTable: "User",
                principalColumn: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Signs_Project_ProjectId",
                table: "Signs");

            migrationBuilder.DropForeignKey(
                name: "FK_Signs_User_ClientId",
                table: "Signs");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Signs",
                newName: "ProjectName");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Signs",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_Signs_ProjectId",
                table: "Signs",
                newName: "IX_Signs_ProjectName");

            migrationBuilder.RenameIndex(
                name: "IX_Signs_ClientId",
                table: "Signs",
                newName: "IX_Signs_Email");

            migrationBuilder.AddColumn<string>(
                name: "ClientEmail",
                table: "Signs",
                type: "nvarchar(256)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "Signs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserEmail",
                table: "Signs",
                type: "nvarchar(256)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectName1",
                table: "Signs",
                type: "nvarchar(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Signs_ClientEmail",
                table: "Signs",
                column: "ClientEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Signs_CreatedByUserEmail",
                table: "Signs",
                column: "CreatedByUserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Signs_ProjectName1",
                table: "Signs",
                column: "ProjectName1");

            migrationBuilder.AddForeignKey(
                name: "FK_Signs_Project_ProjectName",
                table: "Signs",
                column: "ProjectName",
                principalTable: "Project",
                principalColumn: "ProjectName");

            migrationBuilder.AddForeignKey(
                name: "FK_Signs_Project_ProjectName1",
                table: "Signs",
                column: "ProjectName1",
                principalTable: "Project",
                principalColumn: "ProjectName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Signs_User_ClientEmail",
                table: "Signs",
                column: "ClientEmail",
                principalTable: "User",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_Signs_User_CreatedByUserEmail",
                table: "Signs",
                column: "CreatedByUserEmail",
                principalTable: "User",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Signs_User_Email",
                table: "Signs",
                column: "Email",
                principalTable: "User",
                principalColumn: "Email");
        }
    }
}
