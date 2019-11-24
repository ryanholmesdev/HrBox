using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HrBoxApi.Migrations
{
    public partial class tokenexpiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDateUtc",
                table: "UserTokens");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryDateUtc",
                table: "UserTokens",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiryDateUtc",
                table: "UserTokens",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryDateUtc",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "TokenExpiryDateUtc",
                table: "UserTokens");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDateUtc",
                table: "UserTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
