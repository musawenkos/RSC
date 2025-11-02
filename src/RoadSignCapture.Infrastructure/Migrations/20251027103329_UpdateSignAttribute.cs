using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadSignCapture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSignAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcavationCubicM",
                table: "Signs");

            migrationBuilder.DropColumn(
                name: "JpgPath",
                table: "Signs");

            migrationBuilder.DropColumn(
                name: "PdfPath",
                table: "Signs");

            migrationBuilder.DropColumn(
                name: "SgxPath",
                table: "Signs");

            migrationBuilder.RenameColumn(
                name: "ExcavationDepthMm",
                table: "Signs",
                newName: "ExcavationDepthCubicM");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExcavationDepthCubicM",
                table: "Signs",
                newName: "ExcavationDepthMm");

            migrationBuilder.AddColumn<double>(
                name: "ExcavationCubicM",
                table: "Signs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "JpgPath",
                table: "Signs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PdfPath",
                table: "Signs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SgxPath",
                table: "Signs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }
    }
}
