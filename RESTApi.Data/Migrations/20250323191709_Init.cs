using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RESTApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Identyfikator")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false, comment: "Imię"),
                    LastName = table.Column<string>(type: "text", nullable: false, comment: "Nazwisko"),
                    Specialization = table.Column<string>(type: "text", nullable: false, comment: "Specjalizacja"),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "Data utworzenia wpisu"),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "Data ostatniej aktualizacji wpisu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doctors");
        }
    }
}
