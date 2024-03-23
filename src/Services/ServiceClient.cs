
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Transactions;
using Dapper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;
using Npgsql;
using src.DTOs;
using src.Models;

namespace src.Services;

public class ServiceClient
{

    private readonly IConfiguration _configuration;
    public ServiceClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    private static SemaphoreSlim slim = new SemaphoreSlim(1);
    private string connStr = "Host=db;Port=5432;Database=rinha;Username=admin;Password=123;";
    private async Task<IResult> FinalTransaction(int id, RequestTransaction request)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                await connection.OpenAsync();
                var update = request.Type == 'd' ? @"
                                INSERT INTO transacao (valor,id_cliente,tipo_transacao,descricao,realizada_em) 
                                VALUES (@valor,@id_cliente,@tipo_transacao,@descricao,NOW()); 
                                UPDATE cliente SET saldo = saldo - @valor 
                                WHERE id = @d_cliente 
                                AND saldo - @valor >= - limite
                                RETURNING saldo,limite;"
                            : @"
                                INSERT INTO transacao (valor,id_cliente,tipo_transacao,descricao,realizada_em) 
                                VALUES (@valor,@id_cliente,@tipo_transacao,@descricao,NOW()); 

                                UPDATE cliente SET saldo = saldo - @valor 
                                WHERE id = @id_cliente 
                                RETURNING saldo,limite;
                                ";

                using (var cmd = new NpgsqlCommand(update, connection))
                {
                    var transaction = new Transactions(request.Value, request.Type, request.Description);
                    cmd.Parameters.AddWithValue("@valor", transaction.Value);
                    cmd.Parameters.AddWithValue("@id_cliente", id);
                    cmd.Parameters.AddWithValue("@tipo_transacao", transaction.Type);
                    cmd.Parameters.AddWithValue("@descricao", transaction.Description);

                    using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        var balance = reader.GetInt32(0);
                        var limit = reader.GetInt32(1);
                        return Results.Ok(new
                        {
                            limite = limit,
                            saldo = balance
                        });
                    }
                    return Results.UnprocessableEntity();
                }
            }
        }
        catch
        {
            return Results.UnprocessableEntity();
        }
    }

    public async Task<IResult> PerformTransaction(int id, RequestTransaction request)
    {
        if (id < 1 || id > 5)
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
        await slim.WaitAsync();
        try
        {
            return await FinalTransaction(id, request);
        }
        finally
        {
            slim.Release();
        }
    }
    public async Task<IResult> GetExtract(int id)
    {
        if (id < 1 || id > 5)
        {
            return Results.NotFound("Não Existe");
        }

        using (var connection = new NpgsqlConnection(connStr))
        {
            await connection.OpenAsync();
            var command = @"SELECT saldo, limite FROM cliente WHERE id = @id;
                            SELECT valor, tipo_transacao, descricao, realizada_em FROM transacao WHERE id_cliente = @id ORDER BY realizada_em DESC LIMIT 10;";

            using (var responses = await connection.QueryMultipleAsync(command, new { id = id }))
            {
                var saldo = await responses.ReadFirstAsync<Balance>();
                var list_transactions = await responses.ReadAsync<ResponseTransactions>();
                var ultimas_transacoes = new List<ResponseTransactions>();
                var js = new
                {
                    saldo = new ExtractResponse(saldo.saldo, saldo.limite, DateTime.UtcNow),
                    ultimas_transacoes = list_transactions
                };
                return Results.Ok(js);
            };

            /*
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                // await cmd.PrepareAsync().ConfigureAwait(false);
                var read = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                if (!await read.ReadAsync())
                {
                    return Results.NotFound();
                }

                var balance = read.GetInt32(0);
                var limit = read.GetInt32(1);
                var list = new List<ResponseTransactions>();
                list.Add(new ResponseTransactions
                {
                    Value = read.GetInt32(2),
                    Type = read.GetChar(3),
                    Description = read.GetString(4),
                    Hour = read.GetDateTime(5)
                });

                read.Close();
                var balanceCurrent = new Balance
                {
                    total = balance,
                    limit = limit,
                    Date = DateTime.UtcNow
                };
                var responses = new ExtractResponse
                {
                    Balance = balanceCurrent,
                    Latest_Transactions = list
                };

                return Results.Ok(responses);
            }   
                */
        }
    }
}



