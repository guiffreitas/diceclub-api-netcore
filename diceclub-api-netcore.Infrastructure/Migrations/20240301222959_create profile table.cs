using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diceclub_api_netcore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createprofiletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "user_profile",
                newName: "UpdateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "user_profile",
                newName: "UpdateDate");
        }
    }
}
