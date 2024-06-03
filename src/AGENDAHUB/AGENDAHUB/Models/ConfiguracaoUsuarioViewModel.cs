using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AGENDAHUB.Models
{
    public class ConfiguracaoUsuarioViewModel
    {

        public Usuario Usuario { get; set; }
        public Configuracao Configuracao { get; set; }

        public IFormFile Imagem { get; set; }
    }
}
