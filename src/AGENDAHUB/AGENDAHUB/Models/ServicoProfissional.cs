namespace AGENDAHUB.Models
{
    public class ServicoProfissional
    {
        public int ID_Servico { get; set; }
        public Servico Servico { get; set; }
        public int ID_Profissional { get; set; }
        public Profissional Profissional { get; set; }
    }
}
