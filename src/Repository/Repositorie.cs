
using System.Runtime.CompilerServices;
using System.Transactions;
using Microsoft.Extensions.Options;
using src.Models;

namespace src.Repository
{
    public class Repositorie : IRepositorie
    {
        public HashSet<Client> listClients = new()
        {
            new Client(1,100000,0),
            new Client(2,80000,0),
            new Client(3,1000000,0),
            new Client(4,10000000,0),
            new Client(5,500000,0),

        };

        public List<Transactions> transactions = new();
        public async Task Add(Transactions transaction, uint id)
        {
            var tasks = new List<Task>();
            Task add = Task.Run(() => transactions.Add(transaction));
            Task addtransactionInClient = Task.Run(async () =>
            {
                if (listClients.Any(c => c.Id.Equals(id)))
                {
                    var client = await Search(id);
                    client.transactions.Add(transaction);
                }
            });
            tasks.Add(add);
            tasks.Add(addtransactionInClient);
            await Task.WhenAll(tasks);
        }

        public async Task<bool> Exist(uint id)
        {
            Task<bool> exist = Task.Run(() => listClients.Any(d => d.Id.Equals(id)));
            return await exist;
        }

        public async Task<Client> Search(uint id)
        {
            Task<Client> search = Task.Run(() => listClients
            .Where(d => d.Id.Equals(id))
            .First());

            return await search;
        }
        public async Task UpdateBalance(uint id, int value)
        {
            Task up = Task.Run(() =>
            {
                foreach (var cli in listClients.Where(c => c.Id.Equals(id)))
                {
                    cli.Balance -= value;
                }
            });
            await up;
        }
    }
}