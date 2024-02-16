using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public int Limite { get; set; }
        public Extrato? Extrato { get; set; }
    }
}