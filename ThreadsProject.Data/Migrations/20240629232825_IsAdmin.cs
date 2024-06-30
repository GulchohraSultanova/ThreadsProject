using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreadsProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdminOrUser",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminOrUser",
                table: "AspNetUsers");
        }
    }
}
