using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductivityMonitor.Service.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecordDate = table.Column<DateTime>(nullable: false),
                    Pid = table.Column<int>(nullable: false),
                    ApplicationName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeyboardInputRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecordDate = table.Column<DateTime>(nullable: false),
                    KeyPressed = table.Column<string>(nullable: true),
                    ActiveApplicationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyboardInputRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyboardInputRecords_ApplicationRecords_ActiveApplicationId",
                        column: x => x.ActiveApplicationId,
                        principalTable: "ApplicationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MouseInputRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecordDate = table.Column<DateTime>(nullable: false),
                    XPosition = table.Column<int>(nullable: false),
                    YPosition = table.Column<int>(nullable: false),
                    IsClick = table.Column<bool>(nullable: false),
                    ActiveApplicationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MouseInputRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MouseInputRecords_ApplicationRecords_ActiveApplicationId",
                        column: x => x.ActiveApplicationId,
                        principalTable: "ApplicationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyboardInputRecords_ActiveApplicationId",
                table: "KeyboardInputRecords",
                column: "ActiveApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MouseInputRecords_ActiveApplicationId",
                table: "MouseInputRecords",
                column: "ActiveApplicationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyboardInputRecords");

            migrationBuilder.DropTable(
                name: "MouseInputRecords");

            migrationBuilder.DropTable(
                name: "ApplicationRecords");
        }
    }
}
