using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Management_and_Ticket_Booking_System.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
