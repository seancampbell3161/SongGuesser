using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.API.migrations
{
    /// <inheritdoc />
    public partial class AddSongOfTheDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                table: "UserScores",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "SongsOfTheDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Artist = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Song = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongsOfTheDay", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SongsOfTheDay");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "UserScores");
        }
    }
}
