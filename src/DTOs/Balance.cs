using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.DTOs;

public record Balance
{
        public int saldo { get; set; }
        public int limite { get; set; }
}