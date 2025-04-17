using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationMVC.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Groups_GroupId1",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_UserId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_GroupId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_UserId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Students");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId1",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_GroupId1",
                table: "Students",
                column: "GroupId1");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId1",
                table: "Students",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Groups_GroupId1",
                table: "Students",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_UserId1",
                table: "Students",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
