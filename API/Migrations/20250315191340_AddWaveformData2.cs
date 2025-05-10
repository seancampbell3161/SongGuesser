using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.API.migrations
{
    /// <inheritdoc />
    public partial class AddWaveformData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaveformData",
                table: "Tracks");

            migrationBuilder.AddColumn<int>(
                name: "WaveformId",
                table: "Tracks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WaveformData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaveformData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_WaveformId",
                table: "Tracks",
                column: "WaveformId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_WaveformData_WaveformId",
                table: "Tracks",
                column: "WaveformId",
                principalTable: "WaveformData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_WaveformData_WaveformId",
                table: "Tracks");

            migrationBuilder.DropTable(
                name: "WaveformData");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_WaveformId",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "WaveformId",
                table: "Tracks");

            migrationBuilder.AddColumn<string>(
                name: "WaveformData",
                table: "Tracks",
                type: "TEXT",
                nullable: true);
        }
    }
}
