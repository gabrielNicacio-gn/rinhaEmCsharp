using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace src.DTOs;

[JsonSerializable(typeof(ResponseTransactions))]
[JsonSerializable(typeof(ExtractResponse))]
[JsonSerializable(typeof(Balance))]
public partial class JsonContext : JsonSerializerContext { }
