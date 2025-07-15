using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addsomenewcolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentTitle",
                table: "Assignments");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmitDate",
                table: "UserAssignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmitDate",
                table: "UserAssignments");

            migrationBuilder.AddColumn<string>(
                name: "AssignmentTitle",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
