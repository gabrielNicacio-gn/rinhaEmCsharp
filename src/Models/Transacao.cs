using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public int Valor { get; set; }
        public TipoDeTransacao Tipo { get; set; }
        public string? Descricao { get; set; }
        public int ClienteId { get; set; }
        public DateTime DataTransacao { get; set; }
    }
}