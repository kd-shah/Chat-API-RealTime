using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatApi.Migrations
{
    /// <inheritdoc />
    public partial class FileTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    fileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fileSize = table.Column<long>(type: "bigint", nullable: false),
                    contentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uploadDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    senderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    receiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    filePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.fileId);
                    table.ForeignKey(
                        name: "FK_Files_AspNetUsers_receiverId",
                        column: x => x.receiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Files_AspNetUsers_senderId",
                        column: x => x.senderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_receiverId",
                table: "Files",
                column: "receiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_senderId",
                table: "Files",
                column: "senderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
