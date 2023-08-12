using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bnfour.TgBots.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatMacros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Caption = table.Column<string>(type: "TEXT", nullable: false),
                    MediaId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatMacros", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatMacros_Caption",
                table: "CatMacros",
                column: "Caption",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatMacros_MediaId",
                table: "CatMacros",
                column: "MediaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatMacros");
        }
    }
}
