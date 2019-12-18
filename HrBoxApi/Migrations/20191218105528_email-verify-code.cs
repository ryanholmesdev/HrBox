using Microsoft.EntityFrameworkCore.Migrations;

namespace HrBoxApi.Migrations
{
    public partial class emailverifycode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailVerifyCode",
                table: "Users",
                maxLength: 5,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerifyCode",
                table: "Users");
        }
    }
}
