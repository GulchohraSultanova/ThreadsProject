using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreadsProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supports_AspNetUsers_AdminId",
                table: "Supports");

            migrationBuilder.DropIndex(
                name: "IX_Supports_AdminId",
                table: "Supports");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Supports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "Supports",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Supports_AdminId",
                table: "Supports",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Supports_AspNetUsers_AdminId",
                table: "Supports",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
