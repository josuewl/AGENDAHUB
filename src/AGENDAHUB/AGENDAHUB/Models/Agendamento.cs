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

        [Required(ErrorMessage = "Obrigat�rio informar o servi�o!")]
        [Display(Name = "Servi�o")]
        public int ID_Servico { get; set; }

        [Required(ErrorMessage = "Obrigat�rio informar o cliente!")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Obrigat�rio informar a data!")]
        [Column(TypeName = "date")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Obrigat�rio informar a hora!")]
        [Column(TypeName = "time")]
        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "Obrigat�rio informar o status!")]
        public StatusAgendamento Status { get; set; }

        [Required(ErrorMessage = "Informe o Profissional!")]
        [Display(Name = "Profissional")]
        public int ID_Profissional { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        // Propriedades de navega��o
        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        [ForeignKey("ID_Servico")]
        public Servico Servico { get; set; }

        [ForeignKey("ID_Profissional")]
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
