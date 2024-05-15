using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dummy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleToMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Position",
                schema: "member",
                table: "Member",
                type: "text",
                nullable: true,
                defaultValue: "newly_created",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "member",
                table: "Member",
                type: "text",
                nullable: false,
                defaultValue: "user");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Role",
                schema: "member",
                table: "Member",
                column: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Member_Role",
                schema: "member",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "member",
                table: "Member");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                schema: "member",
                table: "Member",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldDefaultValue: "newly_created");
        }
    }
}
