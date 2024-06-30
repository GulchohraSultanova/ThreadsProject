using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreadsProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommentReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReplyUsername",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyUsername",
                table: "Comments");
        }
    }
}
