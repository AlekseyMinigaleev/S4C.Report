using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class addUserModelandYandexGamesAccountModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "YandexGamesAccountId",
                table: "Game",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YandexGamesAccount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeveloperPageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YandexGamesAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YandexGamesAccount_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_YandexGamesAccountId",
                table: "Game",
                column: "YandexGamesAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_YandexGamesAccount_UserId",
                table: "YandexGamesAccount",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_YandexGamesAccount_YandexGamesAccountId",
                table: "Game",
                column: "YandexGamesAccountId",
                principalTable: "YandexGamesAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_YandexGamesAccount_YandexGamesAccountId",
                table: "Game");

            migrationBuilder.DropTable(
                name: "YandexGamesAccount");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Game_YandexGamesAccountId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "YandexGamesAccountId",
                table: "Game");
        }
    }
}
