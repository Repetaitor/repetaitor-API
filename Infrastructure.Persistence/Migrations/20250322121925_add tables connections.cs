using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addtablesconnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RepetaitorGroupId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RepetaitorGroupId",
                table: "Users",
                column: "RepetaitorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignments_AssignmentId",
                table: "UserAssignments",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignments_StatusId",
                table: "UserAssignments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "UserAssignments_AssignmentId",
                table: "UserAssignments",
                columns: new[] { "UserId", "AssignmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OwnerId",
                table: "Groups",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralComments_UserAssignmentId",
                table: "GeneralComments",
                column: "UserAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationTextComments_StatusId",
                table: "EvaluationTextComments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationTextComments_UserAssignmentId",
                table: "EvaluationTextComments",
                column: "UserAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Essays_CreatorId",
                table: "Essays",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CreatorId",
                table: "Assignments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_EssayId",
                table: "Assignments",
                column: "EssayId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_GroupId",
                table: "Assignments",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Essays_EssayId",
                table: "Assignments",
                column: "EssayId",
                principalTable: "Essays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Groups_GroupId",
                table: "Assignments",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_CreatorId",
                table: "Assignments",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Essays_Users_CreatorId",
                table: "Essays",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationTextComments_EvaluationCommentsStatuses_StatusId",
                table: "EvaluationTextComments",
                column: "StatusId",
                principalTable: "EvaluationCommentsStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationTextComments_UserAssignments_UserAssignmentId",
                table: "EvaluationTextComments",
                column: "UserAssignmentId",
                principalTable: "UserAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralComments_UserAssignments_UserAssignmentId",
                table: "GeneralComments",
                column: "UserAssignmentId",
                principalTable: "UserAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_OwnerId",
                table: "Groups",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_AssignmentStatuses_StatusId",
                table: "UserAssignments",
                column: "StatusId",
                principalTable: "AssignmentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_Assignments_AssignmentId",
                table: "UserAssignments",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_Users_UserId",
                table: "UserAssignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Groups_RepetaitorGroupId",
                table: "Users",
                column: "RepetaitorGroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Essays_EssayId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Groups_GroupId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_CreatorId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Essays_Users_CreatorId",
                table: "Essays");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationTextComments_EvaluationCommentsStatuses_StatusId",
                table: "EvaluationTextComments");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationTextComments_UserAssignments_UserAssignmentId",
                table: "EvaluationTextComments");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralComments_UserAssignments_UserAssignmentId",
                table: "GeneralComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_OwnerId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_AssignmentStatuses_StatusId",
                table: "UserAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_Assignments_AssignmentId",
                table: "UserAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_Users_UserId",
                table: "UserAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_RepetaitorGroupId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RepetaitorGroupId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserAssignments_AssignmentId",
                table: "UserAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserAssignments_StatusId",
                table: "UserAssignments");

            migrationBuilder.DropIndex(
                name: "UserAssignments_AssignmentId",
                table: "UserAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Groups_OwnerId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_GeneralComments_UserAssignmentId",
                table: "GeneralComments");

            migrationBuilder.DropIndex(
                name: "IX_EvaluationTextComments_StatusId",
                table: "EvaluationTextComments");

            migrationBuilder.DropIndex(
                name: "IX_EvaluationTextComments_UserAssignmentId",
                table: "EvaluationTextComments");

            migrationBuilder.DropIndex(
                name: "IX_Essays_CreatorId",
                table: "Essays");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_CreatorId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_EssayId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_GroupId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "RepetaitorGroupId",
                table: "Users");
        }
    }
}
