using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS_ASSN2_Rachel.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChangeDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPasswordChangeDate",
                table: "AspNetUsers");
        }
    }
}
