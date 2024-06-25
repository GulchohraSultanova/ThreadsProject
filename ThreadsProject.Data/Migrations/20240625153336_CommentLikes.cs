using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreadsProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommentLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentLike_AspNetUsers_UserId",
                table: "CommentLike");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentLike_Comments_CommentId",
                table: "CommentLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentLike",
                table: "CommentLike");

            migrationBuilder.RenameTable(
                name: "CommentLike",
                newName: "CommentLikes");

            migrationBuilder.RenameIndex(
                name: "IX_CommentLike_UserId",
                table: "CommentLikes",
                newName: "IX_CommentLikes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentLike_CommentId",
                table: "CommentLikes",
                newName: "IX_CommentLikes_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLikes_AspNetUsers_UserId",
                table: "CommentLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLikes_Comments_CommentId",
                table: "CommentLikes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentLikes_AspNetUsers_UserId",
                table: "CommentLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentLikes_Comments_CommentId",
                table: "CommentLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentLikes",
                table: "CommentLikes");

            migrationBuilder.RenameTable(
                name: "CommentLikes",
                newName: "CommentLike");

            migrationBuilder.RenameIndex(
                name: "IX_CommentLikes_UserId",
                table: "CommentLike",
                newName: "IX_CommentLike_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentLikes_CommentId",
                table: "CommentLike",
                newName: "IX_CommentLike_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentLike",
                table: "CommentLike",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLike_AspNetUsers_UserId",
                table: "CommentLike",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLike_Comments_CommentId",
                table: "CommentLike",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
