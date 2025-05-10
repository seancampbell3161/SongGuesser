using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.API.migrations
{
    /// <inheritdoc />
    public partial class AddWaveformData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WaveformData",
                table: "Tracks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaveformData",
                table: "Tracks");
        }
    }
}
