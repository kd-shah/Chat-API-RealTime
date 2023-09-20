using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatApi.Migrations
{
    /// <inheritdoc />
    public partial class isReadProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isRead",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isRead",
                table: "Messages");
        }
    }
}
