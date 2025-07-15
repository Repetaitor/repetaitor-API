using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Domain.Migrations
{
    /// <inheritdoc />
    public partial class delete_isActiveProp_fromGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Groups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
