using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RedeSocial.Migrations
{
    /// <inheritdoc />
    public partial class Amizade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AmizadePendentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SolicitanteId = table.Column<int>(type: "integer", nullable: false),
                    DestinatarioId = table.Column<int>(type: "integer", nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmizadePendentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AmizadePendentes_Usuarios_DestinatarioId",
                        column: x => x.DestinatarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AmizadePendentes_Usuarios_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Amizades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    AmigoId = table.Column<int>(type: "integer", nullable: false),
                    DataConfirmacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId1 = table.Column<int>(type: "integer", nullable: true),
                    UsuarioId2 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amizades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amizades_Usuarios_AmigoId",
                        column: x => x.AmigoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amizades_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amizades_Usuarios_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Amizades_Usuarios_UsuarioId2",
                        column: x => x.UsuarioId2,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AmizadePendentes_DestinatarioId",
                table: "AmizadePendentes",
                column: "DestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AmizadePendentes_SolicitanteId",
                table: "AmizadePendentes",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Amizades_AmigoId",
                table: "Amizades",
                column: "AmigoId");

            migrationBuilder.CreateIndex(
                name: "IX_Amizades_UsuarioId",
                table: "Amizades",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Amizades_UsuarioId1",
                table: "Amizades",
                column: "UsuarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_Amizades_UsuarioId2",
                table: "Amizades",
                column: "UsuarioId2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AmizadePendentes");

            migrationBuilder.DropTable(
                name: "Amizades");
        }
    }
}
