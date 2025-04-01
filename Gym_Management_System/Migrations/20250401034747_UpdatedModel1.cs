using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GymBranches",
                keyColumn: "BranchId",
                keyValue: 1,
                columns: new[] { "Address", "ContactNumber" },
                values: new object[] { "101 Main St", "555-1001" });

            migrationBuilder.UpdateData(
                table: "GymBranches",
                keyColumn: "BranchId",
                keyValue: 2,
                columns: new[] { "Address", "ContactNumber" },
                values: new object[] { "202 High St", "555-2002" });

            migrationBuilder.InsertData(
                table: "GymBranches",
                columns: new[] { "BranchId", "Address", "BranchName", "ContactNumber" },
                values: new object[,]
                {
                    { 3, "303 East Ave", "Eastside Gym", "555-3003" },
                    { 4, "404 West Blvd", "Westside Gym", "555-4004" },
                    { 5, "505 Central Rd", "Central Gym", "555-5005" }
                });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 2,
                columns: new[] { "BranchId", "Capacity", "RoomName" },
                values: new object[] { 1, 25, "Cardio Room" });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomId", "BranchId", "Capacity", "IsAvailable", "RoomName" },
                values: new object[,]
                {
                    { 3, 2, 30, true, "Weight Room" },
                    { 4, 2, 18, true, "Crossfit Zone" },
                    { 5, 3, 15, true, "Spin Studio" },
                    { 6, 3, 20, true, "Dance Studio" },
                    { 7, 4, 12, true, "HIIT Area" },
                    { 8, 4, 16, true, "Pilates Room" },
                    { 9, 5, 10, true, "Stretch Zone" },
                    { 10, 5, 22, true, "Functional Room" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "GymBranches",
                keyColumn: "BranchId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "GymBranches",
                keyColumn: "BranchId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "GymBranches",
                keyColumn: "BranchId",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "GymBranches",
                keyColumn: "BranchId",
                keyValue: 1,
                columns: new[] { "Address", "ContactNumber" },
                values: new object[] { "123 Main St", "123-456-7890" });

            migrationBuilder.UpdateData(
                table: "GymBranches",
                keyColumn: "BranchId",
                keyValue: 2,
                columns: new[] { "Address", "ContactNumber" },
                values: new object[] { "456 High St", "987-654-3210" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: 2,
                columns: new[] { "BranchId", "Capacity", "RoomName" },
                values: new object[] { 2, 30, "Weightlifting Room" });
        }
    }
}
