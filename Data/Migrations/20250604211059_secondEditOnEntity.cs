using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RizeUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class secondEditOnEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DegreeField",
                table: "Educations",
                newName: "DegreeType");

            migrationBuilder.AddColumn<bool>(
                name: "IsOngoing",
                table: "Projects",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "Educations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOngoing",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Certificates");

            migrationBuilder.RenameColumn(
                name: "DegreeType",
                table: "Educations",
                newName: "DegreeField");
        }
    }
}
