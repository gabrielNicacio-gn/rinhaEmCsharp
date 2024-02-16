using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Models
{
    public class Saldo
    {
        public int Limite { get; set; }
        public int Total { get; set; }
        public DateTime DataExtrato { get; set; }
    }
}