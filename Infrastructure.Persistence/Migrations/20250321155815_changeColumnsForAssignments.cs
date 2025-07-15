using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Domain.Migrations
{
    /// <inheritdoc />
    public partial class changeColumnsForAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEvaluated",
                table: "UserAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "EvaluationTextComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssignmentTitle",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEvaluated",
                table: "UserAssignments");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "EvaluationTextComments");

            migrationBuilder.DropColumn(
                name: "AssignmentTitle",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Assignments");
        }
    }
}
