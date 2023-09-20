using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class add_fields_for_RSYA_integration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Login",
                table: "YandexGamesAccount");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "YandexGamesAccount");

            migrationBuilder.AddColumn<string>(
                name: "AuthorizationToken",
                table: "YandexGamesAccount",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "Evaluation",
                table: "GameStatistic",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CashIncome",
                table: "GameStatistic",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PageId",
                table: "Game",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizationToken",
                table: "YandexGamesAccount");

            migrationBuilder.DropColumn(
                name: "CashIncome",
                table: "GameStatistic");

            migrationBuilder.DropColumn(
                name: "PageId",
                table: "Game");

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "YandexGamesAccount",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "YandexGamesAccount",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Evaluation",
                table: "GameStatistic",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
