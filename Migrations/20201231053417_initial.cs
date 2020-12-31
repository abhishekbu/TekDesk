using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TekDesk.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Query",
                columns: table => new
                {
                    QueryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QState = table.Column<int>(type: "int", nullable: false),
                    Added = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Query", x => x.QueryID);
                    table.ForeignKey(
                        name: "FK_Query_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSubject",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    SubjectID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSubject", x => new { x.EmployeeID, x.SubjectID });
                    table.ForeignKey(
                        name: "FK_EmployeeSubject_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSubject_Subject_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subject",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeNotification",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    QueryID = table.Column<int>(type: "int", nullable: false),
                    Notification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeNotification", x => new { x.EmployeeID, x.QueryID });
                    table.ForeignKey(
                        name: "FK_EmployeeNotification_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeNotification_Query_QueryID",
                        column: x => x.QueryID,
                        principalTable: "Query",
                        principalColumn: "QueryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Solution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Added = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    QueryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solution", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Solution_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solution_Query_QueryID",
                        column: x => x.QueryID,
                        principalTable: "Query",
                        principalColumn: "QueryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Artifact",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SolutionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artifact", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Artifact_Solution_SolutionID",
                        column: x => x.SolutionID,
                        principalTable: "Solution",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artifact_SolutionID",
                table: "Artifact",
                column: "SolutionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeNotification_QueryID",
                table: "EmployeeNotification",
                column: "QueryID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSubject_SubjectID",
                table: "EmployeeSubject",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Query_EmployeeID",
                table: "Query",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Solution_EmployeeID",
                table: "Solution",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Solution_QueryID",
                table: "Solution",
                column: "QueryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Artifact");

            migrationBuilder.DropTable(
                name: "EmployeeNotification");

            migrationBuilder.DropTable(
                name: "EmployeeSubject");

            migrationBuilder.DropTable(
                name: "Solution");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "Query");

            migrationBuilder.DropTable(
                name: "Employee");
        }
    }
}
