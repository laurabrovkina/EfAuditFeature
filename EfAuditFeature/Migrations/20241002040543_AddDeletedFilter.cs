using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitHubCopilotExamles.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_People_IsDeleted",
                table: "People",
                column: "IsDeleted",
                filter: "IsDeleted = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_People_IsDeleted",
                table: "People");
        }
    }
}
