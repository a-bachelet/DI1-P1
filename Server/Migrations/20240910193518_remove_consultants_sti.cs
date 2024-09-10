using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class remove_consultants_sti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_consultants_companies_CompanyId",
                table: "consultants");

            migrationBuilder.DropIndex(
                name: "IX_consultants_CompanyId",
                table: "consultants");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "consultants");

            migrationBuilder.DropColumn(
                name: "ConsultantType",
                table: "consultants");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "consultants");

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Salary = table.Column<int>(type: "integer", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    Skills = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employees_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_employees_games_GameId",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employees_CompanyId",
                table: "employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_employees_GameId",
                table: "employees",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "consultants",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConsultantType",
                table: "consultants",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Salary",
                table: "consultants",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_consultants_CompanyId",
                table: "consultants",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_consultants_companies_CompanyId",
                table: "consultants",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
