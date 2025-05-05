using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Domain.Migrations
{
    /// <inheritdoc />
    public partial class change_constraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Essays_EssayId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_AssignmentStatuses_StatusId",
                table: "UserAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Essays_EssayId",
                table: "Assignments",
                column: "EssayId",
                principalTable: "Essays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_AssignmentStatuses_StatusId",
                table: "UserAssignments",
                column: "StatusId",
                principalTable: "AssignmentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Essays_EssayId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_AssignmentStatuses_StatusId",
                table: "UserAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Essays_EssayId",
                table: "Assignments",
                column: "EssayId",
                principalTable: "Essays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_AssignmentStatuses_StatusId",
                table: "UserAssignments",
                column: "StatusId",
                principalTable: "AssignmentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
