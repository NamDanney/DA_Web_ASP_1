using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DA_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTempRegistrationDataToUserOtps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "UserOtps",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "UserOtps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "UserOtps",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "UserOtps",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "UserOtps");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "UserOtps");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "UserOtps");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "UserOtps");
        }
    }
}
