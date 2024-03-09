
using System.Data.Common;
using System.Net.WebSockets;
using Microsoft.VisualBasic;
using Npgsql;
using src.DTOs;
using src.Models;
using src.Repository;

namespace src.Services
{
    public class ServiceClient
    {
        private readonly IRepositorie _appContext;
        private readonly IConfiguration _configuration;
        public ServiceClient(IRepositorie appContext, IConfiguration configuration)
        {
            _appContext = appContext;
            _configuration = configuration;
        }
        private static SemaphoreSlim slim = new SemaphoreSlim(1);

        private async Task<IResult> FinalTransaction(uint id, RequestTransaction request, Client client, Transactions transaction)
        {

            await _appContext.Add(transaction, id);
            await _appContext.UpdateBalance(id, transaction.Value);

            /*
            using (var connection = new NpgsqlConnection(_configuration.GetConnectionString("ConncetDb")))
            {
                
                await connection.OpenAsync();
                var command = @"BEGIN
                                INSERT INTO transacao (id, tipo, descricao, valor, data) 
                                VALUES (@1,@2,@3,@4,@5) ";
                using (var cmd = new NpgsqlCommand(command, connection))
                {

                }
            
            }
            */
            var js =
             new
             {
                 limite = client.Limit,
                 saldo = client.Balance
             };
            return Results.Ok(js);
        }

        public async Task<IResult> PerformTransaction(uint id, RequestTransaction request)
        {
            if (!await _appContext.Exist(id))
            {
                return Results.NotFound("Não Existe");
            }
            if (request.Type != 'c' && request.Type != 'd'
            || request.Description.Length > 10
            || request.Value <= 0
            || string.IsNullOrEmpty(request.Description))
            {
                return Results.UnprocessableEntity();
            }
            //await slim.WaitAsync();
            try
            {
                var client = await _appContext.Search(id);
                var transaction = new Transactions(request.Value, request.Type, request.Description);
                if (request.Type == 'd')
                {
                    var operation = !(client.Balance - request.Value < -client.Limit)
                    ? await FinalTransaction(id, request, client, transaction)
                    : Results.UnprocessableEntity();
                    return operation;
                }
                return await FinalTransaction(id, request, client, transaction);
            }
            finally
            {
                //slim.Release();
            }
        }

        public async Task<IResult> GetExtract(uint id)
        {
            if (!await _appContext.Exist(id))
            {
                return Results.NotFound("Não Existe");
            }
            var client = await _appContext.Search(id);

            var js = new
            {
                saldo = new
                {
                    total = client.Balance,
                    limite = client.Limit,
                    data_extrato = DateTime.UtcNow
                },
                ultimas_transacoes =
             client.transactions
             .OrderBy(t => t.DateOfTransaction)
             .Select(t => new
             {
                 valor = t.Value,
                 tipo = t.Type,
                 descricacao = t.Description,
                 realizada_em = t.DateOfTransaction
             })
            };
            return Results.Ok(js);
        }

        public async Task<IResult> GetUserById(uint id)
        {
            if (!await _appContext.Exist(id))
            {
                return Results.NotFound();
            }
            return Results.Ok(await _appContext.Search(id));
        }
    }
}