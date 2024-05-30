using FluentAssertions.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGENDAHUB.Models
{
    [Table("Agendamentos")]
    public class Agendamento
    {
        [Key]
        public int IdAgendamento { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o serviço!")]
        [Display(Name = "Serviço")]
        public int IdServico { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o cliente!")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a data!")]
        [Column(TypeName = "date")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a hora!")]
        [Column(TypeName = "time")]
        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o status!")]
        public StatusAgendamento Status { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o profissional!")]
        [Display(Name = "Profissional")]
        public int IdProfissional { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        // Propriedades de navegação
        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        [ForeignKey("IdServico")]
        public Servico Servico { get; set; }

        [ForeignKey("IdProfissional")]
        public Profissional Profissional { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        public Caixa Caixa { get; set; }

        public enum StatusAgendamento
        {
            Pendente,
            Concluido,
            Cancelado
        }
    }
}
