using System.Collections.Generic;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

using Server.Models;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class create_skills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ICollection<LeveledSkill>>(
                name: "Skills",
                table: "employees",
                type: "jsonb",
                nullable: false,
                defaultValue: new LeveledSkill[0]);

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "skills",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "HTML" },
                    { 2, "CSS" },
                    { 3, "JavaScript" },
                    { 4, "TypeScript" },
                    { 5, "React" },
                    { 6, "Angular" },
                    { 7, "Vue.js" },
                    { 8, "Node.js" },
                    { 9, "Express.js" },
                    { 10, "ASP.NET Core" },
                    { 11, "Ruby on Rails" },
                    { 12, "Django" },
                    { 13, "Flask" },
                    { 14, "PHP" },
                    { 15, "Laravel" },
                    { 16, "Spring Boot" },
                    { 17, "SQL" },
                    { 18, "NoSQL" },
                    { 19, "GraphQL" },
                    { 20, "REST APIs" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "employees");
        }
    }
}
