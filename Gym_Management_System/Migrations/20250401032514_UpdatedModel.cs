using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GymBranches_GymBranchBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GymBranches_Receptionist_GymBranchBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_GymBranches_GymBranchBranchId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_GymBranchBranchId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_GymBranchBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Receptionist_GymBranchBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "GymBranchBranchId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "GymBranchBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Receptionist_GymBranchBranchId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BranchId",
                table: "Rooms",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BranchId",
                table: "AspNetUsers",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Receptionist_BranchId",
                table: "AspNetUsers",
                column: "Receptionist_BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GymBranches_BranchId",
                table: "AspNetUsers",
                column: "BranchId",
                principalTable: "GymBranches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GymBranches_Receptionist_BranchId",
                table: "AspNetUsers",
                column: "Receptionist_BranchId",
                principalTable: "GymBranches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_GymBranches_BranchId",
                table: "Rooms",
                column: "BranchId",
                principalTable: "GymBranches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GymBranches_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GymBranches_Receptionist_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_GymBranches_BranchId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_BranchId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Receptionist_BranchId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "GymBranchBranchId",
                table: "Rooms",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GymBranchBranchId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Receptionist_GymBranchBranchId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 1,
                column: "GymBranchBranchId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 2,
                column: "GymBranchBranchId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_GymBranchBranchId",
                table: "Rooms",
                column: "GymBranchBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GymBranchBranchId",
                table: "AspNetUsers",
                column: "GymBranchBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Receptionist_GymBranchBranchId",
                table: "AspNetUsers",
                column: "Receptionist_GymBranchBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GymBranches_GymBranchBranchId",
                table: "AspNetUsers",
                column: "GymBranchBranchId",
                principalTable: "GymBranches",
                principalColumn: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GymBranches_Receptionist_GymBranchBranchId",
                table: "AspNetUsers",
                column: "Receptionist_GymBranchBranchId",
                principalTable: "GymBranches",
                principalColumn: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_GymBranches_GymBranchBranchId",
                table: "Rooms",
                column: "GymBranchBranchId",
                principalTable: "GymBranches",
                principalColumn: "BranchId");
        }
    }
}
