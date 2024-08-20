using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class add_rounds_collection_to_games : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE rounds;");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "rounds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_rounds_GameId",
                table: "rounds",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_rounds_games_GameId",
                table: "rounds",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rounds_games_GameId",
                table: "rounds");

            migrationBuilder.DropIndex(
                name: "IX_rounds_GameId",
                table: "rounds");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "rounds");
        }
    }
}
