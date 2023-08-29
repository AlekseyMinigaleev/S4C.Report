using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class splitstatusinGameStatisticModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "GameStatistic");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "GameStatistic");

            migrationBuilder.DropColumn(
                name: "IsPromoted",
                table: "GameStatistic");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GameStatistic",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
