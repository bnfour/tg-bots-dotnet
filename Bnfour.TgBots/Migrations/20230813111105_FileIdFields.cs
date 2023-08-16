using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bnfour.TgBots.Migrations
{
    /// <inheritdoc />
    public partial class FileIdFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MediaId",
                table: "CatMacros",
                newName: "FileUniqueId");

            migrationBuilder.RenameIndex(
                name: "IX_CatMacros_MediaId",
                table: "CatMacros",
                newName: "IX_CatMacros_FileUniqueId");

            migrationBuilder.AddColumn<string>(
                name: "FileId",
                table: "CatMacros",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CatMacros_FileId",
                table: "CatMacros",
                column: "FileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CatMacros_FileId",
                table: "CatMacros");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "CatMacros");

            migrationBuilder.RenameColumn(
                name: "FileUniqueId",
                table: "CatMacros",
                newName: "MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_CatMacros_FileUniqueId",
                table: "CatMacros",
                newName: "IX_CatMacros_MediaId");
        }
    }
}
