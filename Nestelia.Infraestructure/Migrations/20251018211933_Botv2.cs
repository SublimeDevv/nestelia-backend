using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nestelia.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Botv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BotConfigurations",
                table: "BotConfigurations");

            migrationBuilder.RenameTable(
                name: "BotConfigurations",
                newName: "bot_configurations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bot_configurations",
                table: "bot_configurations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_bot_configurations",
                table: "bot_configurations");

            migrationBuilder.RenameTable(
                name: "bot_configurations",
                newName: "BotConfigurations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BotConfigurations",
                table: "BotConfigurations",
                column: "Id");
        }
    }
}
