using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using src.Models;

namespace src.Repository
{
    public interface IRepositorie
    {
        Task Add(Transactions transaction,uint id);
        Task<bool> Exist(uint id);
        Task<Client> Search(uint id);
        Task UpdateBalance(uint id,int value);
    }
}