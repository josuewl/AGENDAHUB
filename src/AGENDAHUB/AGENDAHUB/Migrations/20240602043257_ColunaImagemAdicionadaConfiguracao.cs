using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGENDAHUB.Migrations
{
    public partial class ColunaImagemAdicionadaConfiguracao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Usuarios");

            migrationBuilder.AddColumn<byte[]>(
                name: "Imagem",
                table: "Configuracao",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Configuracao");

            migrationBuilder.AddColumn<byte[]>(
                name: "Imagem",
                table: "Usuarios",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
