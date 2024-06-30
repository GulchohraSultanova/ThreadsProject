using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreadsProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditUserAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Requests_RequestId",
                table: "Actions");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "Actions",
                newName: "FollowRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_RequestId",
                table: "Actions",
                newName: "IX_Actions_FollowRequestId");

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Actions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Actions_CommentId",
                table: "Actions",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Comments_CommentId",
                table: "Actions",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Requests_FollowRequestId",
                table: "Actions",
                column: "FollowRequestId",
                principalTable: "Requests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Comments_CommentId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Requests_FollowRequestId",
                table: "Actions");

            migrationBuilder.DropIndex(
                name: "IX_Actions_CommentId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Actions");

            migrationBuilder.RenameColumn(
                name: "FollowRequestId",
                table: "Actions",
                newName: "RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_FollowRequestId",
                table: "Actions",
                newName: "IX_Actions_RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Requests_RequestId",
                table: "Actions",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
