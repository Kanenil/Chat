using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblMessage_tblUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblMessageUsers",
                columns: table => new
                {
                    UserFromId = table.Column<int>(type: "int", nullable: false),
                    UserToId = table.Column<int>(type: "int", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMessageUsers", x => new { x.UserToId, x.UserFromId, x.MessageId });
                    table.ForeignKey(
                        name: "FK_tblMessageUsers_tblMessage_MessageId",
                        column: x => x.MessageId,
                        principalTable: "tblMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_tblMessageUsers_tblUsers_UserFromId",
                        column: x => x.UserFromId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_tblMessageUsers_tblUsers_UserToId",
                        column: x => x.UserToId,
                        principalTable: "tblUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblMessage_UserId",
                table: "tblMessage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tblMessageUsers_MessageId",
                table: "tblMessageUsers",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_tblMessageUsers_UserFromId",
                table: "tblMessageUsers",
                column: "UserFromId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblMessageUsers");

            migrationBuilder.DropTable(
                name: "tblMessage");

            migrationBuilder.DropTable(
                name: "tblUsers");
        }
    }
}
