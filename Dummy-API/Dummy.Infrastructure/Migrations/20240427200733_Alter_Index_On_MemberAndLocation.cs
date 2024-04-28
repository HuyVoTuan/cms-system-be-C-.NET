using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dummy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Alter_Index_On_MemberAndLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Member_FirstName",
                schema: "member",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_LastName",
                schema: "member",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Location_City",
                schema: "location",
                table: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Slug",
                schema: "member",
                table: "Member",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Location_District",
                schema: "location",
                table: "Location",
                column: "District");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Member_Slug",
                schema: "member",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Location_District",
                schema: "location",
                table: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Member_FirstName",
                schema: "member",
                table: "Member",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Member_LastName",
                schema: "member",
                table: "Member",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Location_City",
                schema: "location",
                table: "Location",
                column: "City");
        }
    }
}
