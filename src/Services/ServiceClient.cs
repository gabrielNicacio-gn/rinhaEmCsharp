
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using src.DTOs;
using src.Models;
using src.Repository;

namespace src.Services
{
    public class ServiceClient
    {
        private readonly IRepositorie _appContext;
        public ServiceClient(IRepositorie appContext)
        {
            _appContext = appContext;
        }

        public async Task<IResult> Debit(uint id, RequestTransaction request, Client client, Transactions transaction)
        {
            if (client.Balance - request.Value < 0 - client.Limit)
            {
                return Results.UnprocessableEntity();
            }
            await _appContext.Add(transaction, id);
            await _appContext.UpdateBalance(id, transaction.Value);
            var js =
             new
             {
                 limite = client.Limit,
                 saldo = client.Balance
             };
            return Results.Ok(js);
        }
        public async Task<IResult> Credit(uint id, RequestTransaction request, Client client, Transactions transaction)
        {
            await _appContext.Add(transaction, id);
            await _appContext.UpdateBalance(id, transaction.Value);
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

            var client = await _appContext.Search(id);
            var transaction = new Transactions(request.Value, request.Type, request.Description);
            if (request.Type == TypeOfTransaction.Debit)
            {
                return await Debit(id, request, client, transaction);
            }
            return await Credit(id, request, client, transaction);
        }

        public async Task<IResult> ConsultarExtrato(uint id)
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
                 tipo = t.Type.ToString(),
                 descricacao = t.Description,
                 realizada_em = t.DateOfTransaction
             })
            };
            return Results.Ok(js);
        }
    }
}