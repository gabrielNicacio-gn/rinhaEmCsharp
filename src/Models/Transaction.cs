using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace src.Models
{
    public class Transactions
    {
        [JsonIgnore]
        public uint Id { get; init; }
        public int Value { get; set; }
        public TypeOfTransaction Type { get; set; }
        public string Description { get; set; } = string.Empty;
        [JsonIgnore]
        public Client? Client { get; set; }
        public DateTime DateOfTransaction { get; set; }

        public Transactions(int value,TypeOfTransaction type,string description)
        {
            Value = value;
            Type = type;
            Description = description;
            DateOfTransaction = DateTime.UtcNow;
        }
    }
}