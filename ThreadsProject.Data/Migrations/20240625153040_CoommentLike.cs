﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreadsProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class CoommentLike : Migration
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentLike_AspNetUsers_UserId",
                table: "CommentLike");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentLike_Comments_CommentId",
                table: "CommentLike");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLike_AspNetUsers_UserId",
                table: "CommentLike",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLike_Comments_CommentId",
                table: "CommentLike",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
