using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using src.Models;

namespace src.DTOs
{
    public record RespostasExtrato(Saldo Saldo,List<Transacao> UltimasTransacoes);
}