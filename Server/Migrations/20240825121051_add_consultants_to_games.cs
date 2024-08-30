using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class add_consultants_to_games : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "consultants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_consultants_GameId",
                table: "consultants",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_consultants_games_GameId",
                table: "consultants",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_consultants_games_GameId",
                table: "consultants");

            migrationBuilder.DropIndex(
                name: "IX_consultants_GameId",
                table: "consultants");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "consultants");
        }
    }
}
