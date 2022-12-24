using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class createtblMessageUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
