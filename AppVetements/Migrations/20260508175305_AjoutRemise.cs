using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppVetements.Migrations
{
    /// <inheritdoc />
    public partial class AjoutRemise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Remise",
                table: "Produits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remise",
                table: "Produits");
        }
    }
}
