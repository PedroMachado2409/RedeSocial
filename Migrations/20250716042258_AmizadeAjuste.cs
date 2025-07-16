using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedeSocial.Migrations
{
    /// <inheritdoc />
    public partial class AmizadeAjuste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PedidoId",
                table: "Amizades",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Amizades_PedidoId",
                table: "Amizades",
                column: "PedidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Amizades_AmizadePendentes_PedidoId",
                table: "Amizades",
                column: "PedidoId",
                principalTable: "AmizadePendentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amizades_AmizadePendentes_PedidoId",
                table: "Amizades");

            migrationBuilder.DropIndex(
                name: "IX_Amizades_PedidoId",
                table: "Amizades");

            migrationBuilder.DropColumn(
                name: "PedidoId",
                table: "Amizades");
        }
    }
}
