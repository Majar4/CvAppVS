using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvAppen.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedByUserToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedByUserId",
                value: "5f8dedbb-0023-4225-90a3-fb982dde34e4");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedByUserId",
                value: "5f8dedbb-0023-4225-90a3-fb982dde34e4");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedByUserId",
                table: "Projects",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedByUserId",
                table: "Projects",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedByUserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CreatedByUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5f8dedbb-0023-4225-90a3-fb982dde34e4",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "81b9b94a-0571-4f84-9909-e4766303a40c", "54815b6a-2a13-4c39-861d-ffe80a2f0f25" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70d91398-1e9f-4c6c-9464-31629296e124",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "f5e80f5e-bdac-4ae7-b07f-c90f5fabcee4", "5d2817a4-1db3-438e-b56f-125c8569c84f" });
        }
    }
}
