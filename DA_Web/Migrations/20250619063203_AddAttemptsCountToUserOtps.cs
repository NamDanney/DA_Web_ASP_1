using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DA_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAttemptsCountToUserOtps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Attempts",
                table: "UserOtps",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attempts",
                table: "UserOtps");
        }
    }
}
