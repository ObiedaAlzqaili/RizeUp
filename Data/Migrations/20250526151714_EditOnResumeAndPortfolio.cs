using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RizeUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditOnResumeAndPortfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Resumes",
                type: "bit",
                nullable: true,
                defaultValue:false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Portfolios",
                type: "bit",
                nullable: true,
                defaultValue:false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Portfolios");
        }
    }
}
