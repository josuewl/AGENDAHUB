using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGENDAHUB.Migrations
{
    public partial class novoNomeColunasAgendamentos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Profissionais_IdProfissional",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_IdServico",
                table: "Agendamentos");

            migrationBuilder.RenameColumn(
                name: "IdServico",
                table: "Agendamentos",
                newName: "ID_Servico");

            migrationBuilder.RenameColumn(
                name: "IdProfissional",
                table: "Agendamentos",
                newName: "ID_Profissional");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_IdServico",
                table: "Agendamentos",
                newName: "IX_Agendamentos_ID_Servico");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_IdProfissional",
                table: "Agendamentos",
                newName: "IX_Agendamentos_ID_Profissional");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Profissionais_ID_Profissional",
                table: "Agendamentos",
                column: "ID_Profissional",
                principalTable: "Profissionais",
                principalColumn: "ID_Profissional",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_ID_Servico",
                table: "Agendamentos",
                column: "ID_Servico",
                principalTable: "Servicos",
                principalColumn: "ID_Servico",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Profissionais_ID_Profissional",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_ID_Servico",
                table: "Agendamentos");

            migrationBuilder.RenameColumn(
                name: "ID_Servico",
                table: "Agendamentos",
                newName: "IdServico");

            migrationBuilder.RenameColumn(
                name: "ID_Profissional",
                table: "Agendamentos",
                newName: "IdProfissional");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_ID_Servico",
                table: "Agendamentos",
                newName: "IX_Agendamentos_IdServico");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_ID_Profissional",
                table: "Agendamentos",
                newName: "IX_Agendamentos_IdProfissional");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Profissionais_IdProfissional",
                table: "Agendamentos",
                column: "IdProfissional",
                principalTable: "Profissionais",
                principalColumn: "ID_Profissional",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_IdServico",
                table: "Agendamentos",
                column: "IdServico",
                principalTable: "Servicos",
                principalColumn: "ID_Servico",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
