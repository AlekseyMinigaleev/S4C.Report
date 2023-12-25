using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class Update_AuthorizationToken_on_RsyaAythorizationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorizationToken",
                table: "User",
                newName: "RsyaAuthorizationToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RsyaAuthorizationToken",
                table: "User",
                newName: "AuthorizationToken");
        }
    }
}
