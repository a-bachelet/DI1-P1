using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class update_game_status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusBis",
                table: "games",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "Waiting");

            migrationBuilder.Sql("UPDATE games SET \"StatusBis\" = \"Status\"::TEXT;");

            migrationBuilder.DropColumn("Status", "games");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "games",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "Waiting");

            migrationBuilder.Sql("UPDATE games SET \"Status\" = \"StatusBis\";");

            migrationBuilder.DropColumn("StatusBis", "games");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:public.game_status", "Waiting,InProgress,Finished");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.game_status", "Waiting,InProgress,Finished");

            migrationBuilder.AddColumn<string>(
                name: "StatusBis",
                table: "games",
                type: "game_status",
                nullable: false,
                defaultValue: "Waiting");

            migrationBuilder.Sql("UPDATE games SET \"StatusBis\" = \"Status\"::game_status;");

            migrationBuilder.DropColumn("Status", "games");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "games",
                type: "game_status",
                nullable: false,
                defaultValue: "Waiting");

            migrationBuilder.Sql("UPDATE games SET \"Status\" = \"StatusBis\";");

            migrationBuilder.DropColumn("StatusBis", "games");
        }
    }
}
