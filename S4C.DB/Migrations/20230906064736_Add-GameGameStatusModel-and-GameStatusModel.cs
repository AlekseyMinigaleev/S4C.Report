using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class AddGameGameStatusModelandGameStatusModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "GameStatistic");

            migrationBuilder.DropColumn(
                name: "IsPromoted",
                table: "GameStatistic");

            migrationBuilder.CreateTable(
                name: "GameStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameGameStatus",
                columns: table => new
                {
                    GameStatisticId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGameStatus", x => new { x.GameStatisticId, x.GameStatusId });
                    table.ForeignKey(
                        name: "FK_GameGameStatus_GameStatistic_GameStatisticId",
                        column: x => x.GameStatisticId,
                        principalTable: "GameStatistic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGameStatus_GameStatus_GameStatusId",
                        column: x => x.GameStatusId,
                        principalTable: "GameStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameGameStatus_GameStatusId",
                table: "GameGameStatus",
                column: "GameStatusId");

            migrationBuilder.Sql(
                "INSERT INTO GameStatus (Id, Name) " +
                "VALUES " +
                    "(NEWID(), 'new'), " +
                    "(NEWID(), 'promoted');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameGameStatus");

            migrationBuilder.DropTable(
                name: "GameStatus");

            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "GameStatistic",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPromoted",
                table: "GameStatistic",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
