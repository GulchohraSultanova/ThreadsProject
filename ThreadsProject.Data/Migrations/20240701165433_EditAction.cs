using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreadsProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Requests_FollowRequestId",
                table: "Actions");

            migrationBuilder.DropIndex(
                name: "IX_Actions_FollowRequestId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "FollowRequestId",
                table: "Actions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FollowRequestId",
                table: "Actions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Actions_FollowRequestId",
                table: "Actions",
                column: "FollowRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Requests_FollowRequestId",
                table: "Actions",
                column: "FollowRequestId",
                principalTable: "Requests",
                principalColumn: "Id");
        }
    }
}
