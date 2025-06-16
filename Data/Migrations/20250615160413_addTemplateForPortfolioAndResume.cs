using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RizeUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addTemplateForPortfolioAndResume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResumeTemplateId",
                table: "Resumes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PortfolioTemplateId",
                table: "Portfolios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeTemplateId",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "PortfolioTemplateId",
                table: "Portfolios");
        }
    }
}
