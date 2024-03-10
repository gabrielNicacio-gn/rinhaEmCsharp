using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;

namespace src.DTOs
{
    public record struct ResponseTransactions(int Value, char Type, string Description, DateTime Hour);
}