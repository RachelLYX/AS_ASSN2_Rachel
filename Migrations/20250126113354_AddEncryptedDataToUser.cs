﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS_ASSN2_Rachel.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptedDataToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptedData",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedData",
                table: "AspNetUsers");
        }
    }
}
