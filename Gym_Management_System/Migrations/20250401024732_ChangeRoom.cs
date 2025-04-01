using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GymBranchId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GymBranchId",
                table: "AspNetUsers",
                column: "GymBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GymBranches_GymBranchId",
                table: "AspNetUsers",
                column: "GymBranchId",
                principalTable: "GymBranches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GymBranches_GymBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_GymBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GymBranchId",
                table: "AspNetUsers");
        }
    }
}
