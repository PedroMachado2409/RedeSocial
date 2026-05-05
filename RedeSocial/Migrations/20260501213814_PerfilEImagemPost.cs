using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedeSocial.Migrations
{
    /// <inheritdoc />
    public partial class PerfilEImagemPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescricaoPerfil",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoBanner",
                table: "Usuarios",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoPerfil",
                table: "Usuarios",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImagemPost",
                table: "Posts",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescricaoPerfil",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FotoBanner",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ImagemPost",
                table: "Posts");
        }
    }
}
