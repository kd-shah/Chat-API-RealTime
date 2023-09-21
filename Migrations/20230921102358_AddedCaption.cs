using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedCaption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "caption",
                table: "Files",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "caption",
                table: "Files");
        }
    }
}
