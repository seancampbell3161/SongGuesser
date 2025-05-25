using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.API.migrations
{
    /// <inheritdoc />
    public partial class AddFKrelationshipSongOfTheDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Song",
                table: "SongsOfTheDay",
                newName: "SongTitle");

            migrationBuilder.AddColumn<int>(
                name: "SongId",
                table: "SongsOfTheDay",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SongsOfTheDay_SongId",
                table: "SongsOfTheDay",
                column: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_SongsOfTheDay_Songs_SongId",
                table: "SongsOfTheDay",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SongsOfTheDay_Songs_SongId",
                table: "SongsOfTheDay");

            migrationBuilder.DropIndex(
                name: "IX_SongsOfTheDay_SongId",
                table: "SongsOfTheDay");

            migrationBuilder.DropColumn(
                name: "SongId",
                table: "SongsOfTheDay");

            migrationBuilder.RenameColumn(
                name: "SongTitle",
                table: "SongsOfTheDay",
                newName: "Song");
        }
    }
}
