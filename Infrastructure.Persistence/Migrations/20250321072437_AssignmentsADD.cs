using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AssignmentsADD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_groups",
                table: "groups");

            migrationBuilder.RenameTable(
                name: "groups",
                newName: "Groups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    EssayId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationCommentsStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationCommentsStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationTextComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAssignmentId = table.Column<int>(type: "int", nullable: false),
                    StartIndex = table.Column<int>(type: "int", nullable: false),
                    EndIndex = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationTextComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneralComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAssignmentId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WordCount = table.Column<int>(type: "int", nullable: false),
                    GrammarScore = table.Column<int>(type: "int", nullable: false),
                    FluencyScore = table.Column<int>(type: "int", nullable: false),
                    FeedbackSeen = table.Column<bool>(type: "bit", nullable: false),
                    AssignDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "AssignmentStatuses");

            migrationBuilder.DropTable(
                name: "EvaluationCommentsStatuses");

            migrationBuilder.DropTable(
                name: "EvaluationTextComments");

            migrationBuilder.DropTable(
                name: "GeneralComments");

            migrationBuilder.DropTable(
                name: "UserAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "groups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_groups",
                table: "groups",
                column: "Id");
        }
    }
}
