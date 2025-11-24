using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversitySystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departaments",
                columns: table => new
                {
                    IdDepartament = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departaments", x => x.IdDepartament);
                });

            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    IdDiscipline = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.IdDiscipline);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroups",
                columns: table => new
                {
                    IdGroup = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDepartament = table.Column<int>(type: "int", nullable: true),
                    NumberGroup = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroups", x => x.IdGroup);
                    table.ForeignKey(
                        name: "FK_StudentGroups_Departaments_IdDepartament",
                        column: x => x.IdDepartament,
                        principalTable: "Departaments",
                        principalColumn: "IdDepartament");
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    IdTeacher = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDepartament = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecondName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Patronymic = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Login = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.IdTeacher);
                    table.ForeignKey(
                        name: "FK_Teachers_Departaments_IdDepartament",
                        column: x => x.IdDepartament,
                        principalTable: "Departaments",
                        principalColumn: "IdDepartament");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    IdStudent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdGroup = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecondName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Patronymic = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateBirthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Login = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.IdStudent);
                    table.ForeignKey(
                        name: "FK_Students_StudentGroups_IdGroup",
                        column: x => x.IdGroup,
                        principalTable: "StudentGroups",
                        principalColumn: "IdGroup");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_IdDepartament",
                table: "StudentGroups",
                column: "IdDepartament");

            migrationBuilder.CreateIndex(
                name: "IX_Students_IdGroup",
                table: "Students",
                column: "IdGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_IdDepartament",
                table: "Teachers",
                column: "IdDepartament");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "StudentGroups");

            migrationBuilder.DropTable(
                name: "Departaments");
        }
    }
}
