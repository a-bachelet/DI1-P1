using System.Collections.Generic;

using Microsoft.EntityFrameworkCore.Migrations;

using Server.Models;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class add_treasury_to_companies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Skills",
                table: "employees",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(ICollection<LeveledSkill>),
                oldType: "jsonb",
                oldDefaultValue: new LeveledSkill[0]);

            migrationBuilder.AddColumn<int>(
                name: "Treasury",
                table: "companies",
                type: "integer",
                nullable: false,
                defaultValue: 1000000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Treasury",
                table: "companies");

            migrationBuilder.AlterColumn<ICollection<LeveledSkill>>(
                name: "Skills",
                table: "employees",
                type: "jsonb",
                nullable: false,
                defaultValue: new LeveledSkill[0],
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
