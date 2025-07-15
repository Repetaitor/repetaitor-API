using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Domain.Migrations
{
    /// <inheritdoc />
    public partial class removeunqiueconstraintfromemail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthCodes_Email",
                table: "AuthCodes");

            migrationBuilder.CreateIndex(
                name: "IX_AuthCodes_Email",
                table: "AuthCodes",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthCodes_Email",
                table: "AuthCodes");

            migrationBuilder.CreateIndex(
                name: "IX_AuthCodes_Email",
                table: "AuthCodes",
                column: "Email",
                unique: true);
        }
    }
}
