using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS_ASSN2_Rachel.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHistoryToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHistory",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHistory",
                table: "AspNetUsers");
        }
    }
}
