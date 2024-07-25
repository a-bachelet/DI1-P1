using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class create_consultants_and_move_employees_to_consultants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_companies_CompanyId",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employees",
                table: "employees");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "consultants");

            migrationBuilder.RenameIndex(
                name: "IX_employees_CompanyId",
                table: "consultants",
                newName: "IX_consultants_CompanyId");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "consultants",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "ConsultantType",
                table: "consultants",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE consultants SET \"ConsultantType\" = 'Employee';");

            migrationBuilder.AddPrimaryKey(
                name: "PK_consultants",
                table: "consultants",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_consultants_companies_CompanyId",
                table: "consultants",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_consultants_companies_CompanyId",
                table: "consultants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_consultants",
                table: "consultants");

            migrationBuilder.DropColumn(
                name: "ConsultantType",
                table: "consultants");

            migrationBuilder.RenameTable(
                name: "consultants",
                newName: "employees");

            migrationBuilder.RenameIndex(
                name: "IX_consultants_CompanyId",
                table: "employees",
                newName: "IX_employees_CompanyId");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "employees",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_employees",
                table: "employees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_companies_CompanyId",
                table: "employees",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
