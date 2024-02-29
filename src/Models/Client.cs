using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace src.Models
{
    public class Client
    {
        public uint Id { get; set; }
        public int Limit { get; set; }
        public int Balance { get; set; }
        public List<Transactions> transactions {get;set;}

        public Client(uint id,int limit, int balance)
        {
            Id = id;
            Limit = limit;
            Balance = balance;
            transactions = new();
        }
    }
}