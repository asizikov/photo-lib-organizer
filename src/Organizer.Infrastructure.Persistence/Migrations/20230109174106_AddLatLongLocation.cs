using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organizer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLatLongLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "PhotoFiles",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "PhotoFiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "PhotoFiles",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhotoFiles_Location",
                table: "PhotoFiles",
                column: "Location");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PhotoFiles_Location",
                table: "PhotoFiles");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "PhotoFiles");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "PhotoFiles");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "PhotoFiles");
        }
    }
}
