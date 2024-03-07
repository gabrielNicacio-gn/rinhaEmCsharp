using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using src.Models;

namespace src.DTOs
{
    public record RequestTransaction(int Value, char Type, string Description);
}