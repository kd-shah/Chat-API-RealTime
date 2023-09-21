using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatApi.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFile",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "fileId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "messageId",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_fileId",
                table: "Messages",
                column: "fileId",
                unique: true,
                filter: "[fileId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Files_fileId",
                table: "Messages",
                column: "fileId",
                principalTable: "Files",
                principalColumn: "fileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Files_fileId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_fileId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsFile",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "fileId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "messageId",
                table: "Files");
        }
    }
}
