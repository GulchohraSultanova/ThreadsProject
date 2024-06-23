using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreadsProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditFollowsandRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerUserId",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_Followings_AspNetUsers_FollowingUserId1",
                table: "Followings");

            migrationBuilder.DropIndex(
                name: "IX_Followings_FollowingUserId1",
                table: "Followings");

            migrationBuilder.DropColumn(
                name: "FollowingUserId1",
                table: "Followings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Followings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Followings_UserId",
                table: "Followings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerUserId",
                table: "Followers",
                column: "FollowerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Followings_AspNetUsers_UserId",
                table: "Followings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerUserId",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_Followings_AspNetUsers_UserId",
                table: "Followings");

            migrationBuilder.DropIndex(
                name: "IX_Followings_UserId",
                table: "Followings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Followings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "FollowingUserId1",
                table: "Followings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Followings_FollowingUserId1",
                table: "Followings",
                column: "FollowingUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerUserId",
                table: "Followers",
                column: "FollowerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Followings_AspNetUsers_FollowingUserId1",
                table: "Followings",
                column: "FollowingUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
