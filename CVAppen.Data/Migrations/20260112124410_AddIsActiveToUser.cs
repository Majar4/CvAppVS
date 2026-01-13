using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvAppen.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5f8dedbb-0023-4225-90a3-fb982dde34e4",
                columns: new[] { "ConcurrencyStamp", "IsActive", "SecurityStamp" },
                values: new object[] { "b79a2ea8-b2c4-4f43-811d-93ad54f4c160", true, "1e8c931b-e815-4d33-96c2-e0ff1fb261f1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70d91398-1e9f-4c6c-9464-31629296e124",
                columns: new[] { "ConcurrencyStamp", "IsActive", "SecurityStamp" },
                values: new object[] { "94d2e805-9946-4589-9760-4ec22eb73ce0", true, "e4743e01-ee1f-4a07-8ef3-492537c7129e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5f8dedbb-0023-4225-90a3-fb982dde34e4",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "2587660c-c977-4b60-863e-7e440e79b715", "dffc7180-b10c-429e-9b55-64a10607ac0d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70d91398-1e9f-4c6c-9464-31629296e124",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "3c7a47cd-1021-45b8-81c4-7996abd43726", "9078bb8a-c878-459e-85cb-95fd531e7b6b" });
        }
    }
}
