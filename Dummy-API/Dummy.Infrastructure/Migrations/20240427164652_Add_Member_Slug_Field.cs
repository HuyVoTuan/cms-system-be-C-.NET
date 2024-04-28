using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dummy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Member_Slug_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                schema: "member",
                table: "Member",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                schema: "member",
                table: "Member");
        }
    }
}
