using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace seminar5.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message_text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    message_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_isent = table.Column<bool>(type: "bit", nullable: false),
                    UserToId = table.Column<int>(type: "int", nullable: true),
                    UserFromId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("message_pk", x => x.id);
                    table.ForeignKey(
                        name: "message_From_User_FK ",
                        column: x => x.UserFromId,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "message_To_User_FK ",
                        column: x => x.UserToId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_message_UserFromId",
                table: "message",
                column: "UserFromId");

            migrationBuilder.CreateIndex(
                name: "IX_message_UserToId",
                table: "message",
                column: "UserToId");

            migrationBuilder.CreateIndex(
                name: "IX_users_FullName",
                table: "users",
                column: "FullName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
