using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;

namespace src.DTOs
{
    public record ResponseTransactions(int? valor, char? tipo_transacao, string? descricao, DateTime? realizada_em);
}