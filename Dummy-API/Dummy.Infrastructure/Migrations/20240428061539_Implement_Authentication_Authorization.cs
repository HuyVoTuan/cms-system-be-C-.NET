using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dummy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Implement_Authentication_Authorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "member",
                table: "Member",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                schema: "member",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Member_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "member",
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_ExpiredDate",
                schema: "member",
                table: "RefreshToken",
                column: "ExpiredDate");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_MemberId",
                schema: "member",
                table: "RefreshToken",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                schema: "member",
                table: "RefreshToken",
                column: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken",
                schema: "member");

            migrationBuilder.DropColumn(
                name: "Password",
                schema: "member",
                table: "Member");
        }
    }
}
