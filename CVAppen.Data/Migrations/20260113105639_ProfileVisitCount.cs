using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvAppen.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProfileVisitCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VisitCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5f8dedbb-0023-4225-90a3-fb982dde34e4",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VisitCount" },
                values: new object[] { "dbf0b28f-0b0d-4218-a7ee-5e2c5037a597", "2ec60f05-4eb7-42a4-94b2-1107c6db94cf", 0 });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70d91398-1e9f-4c6c-9464-31629296e124",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VisitCount" },
                values: new object[] { "be2b9ce1-5a2c-47c3-8c57-7befac4edd81", "226dd376-98d8-4c3d-a31a-203d1766e60b", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitCount",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5f8dedbb-0023-4225-90a3-fb982dde34e4",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "b79a2ea8-b2c4-4f43-811d-93ad54f4c160", "1e8c931b-e815-4d33-96c2-e0ff1fb261f1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70d91398-1e9f-4c6c-9464-31629296e124",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "94d2e805-9946-4589-9760-4ec22eb73ce0", "e4743e01-ee1f-4a07-8ef3-492537c7129e" });
        }
    }
}
