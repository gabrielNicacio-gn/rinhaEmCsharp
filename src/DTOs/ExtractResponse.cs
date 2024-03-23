using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace src.DTOs
{
    public record ExtractResponse(int total, int limit, DateTime date);
}