using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_media_feed.Migrations
{
    public partial class ScriptB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Contect",
                table: "Posts",
                newName: "Content");

            migrationBuilder.AddColumn<string>(
                name: "ClaimType",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClaimType",
                table: "Users",
                column: "ClaimType",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ClaimType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClaimType",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Posts",
                newName: "Contect");
        }
    }
}
