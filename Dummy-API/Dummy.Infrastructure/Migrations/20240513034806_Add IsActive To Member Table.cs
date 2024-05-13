using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dummy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "member",
                table: "Member",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "member",
                table: "Member");
        }
    }
}
