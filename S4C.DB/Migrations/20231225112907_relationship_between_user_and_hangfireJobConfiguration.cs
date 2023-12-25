using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class relationship_between_user_and_hangfireJobConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "HangfireJobConfiguration",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_HangfireJobConfiguration_UserId",
                table: "HangfireJobConfiguration",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HangfireJobConfiguration_User_UserId",
                table: "HangfireJobConfiguration",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HangfireJobConfiguration_User_UserId",
                table: "HangfireJobConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_HangfireJobConfiguration_UserId",
                table: "HangfireJobConfiguration");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HangfireJobConfiguration");
        }
    }
}
