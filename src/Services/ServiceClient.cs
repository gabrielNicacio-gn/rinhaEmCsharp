
using System.Data;
using System.Data.Common;
using System.Net.WebSockets;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;
using Npgsql;
using src.DTOs;
using src.Models;

namespace src.Services
{
    public class ServiceClient
    {

        private readonly IConfiguration _configuration;
        public ServiceClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private static SemaphoreSlim slim = new SemaphoreSlim(1);
        private string connStr = "Host=localhost;Database=rinha;Username=admin;Password=123;";
        private async Task<IResult> FinalTransaction(int id, RequestTransaction request)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    var update = request.Type == 'd' ? @"BEGIN
                                INSERT INTO transacoes (valor,id_cliente,tipo_transacao,descricao,realizada_em) 
                                VALUES ($1,$2,$3,$4,$5); 

                                UPDATE clientes SET saldo = saldo - $1 
                                WHERE id = $2 
                                AND saldo - $1 >= - limite
                                RETURNING saldo,limite;
                                END"
                                : @"BEGIN
                                INSERT INTO transacoes (valor,id_cliente,tipo_transacao,descricao,realizada_em) 
                                VALUES ($1,$2,$3,$4,$5); 

                                UPDATE clientes SET saldo = saldo - $1 
                                WHERE id = $2 
                                RETURNING saldo,limite;
                                END";

                    using (var cmd = new NpgsqlCommand(update, connection))
                    {
                        var transaction = new Transactions(request.Value, request.Type, request.Description);
                        cmd.Parameters.AddWithValue("$1", transaction.Value);
                        cmd.Parameters.AddWithValue("$2", id);
                        cmd.Parameters.AddWithValue("$3", transaction.Type);
                        cmd.Parameters.AddWithValue("$4", transaction.Description);
                        cmd.Parameters.AddWithValue("$5", DateTime.UtcNow);

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

            //try
            //{
            using (var connection = new NpgsqlConnection(connStr))
            {
                await connection.OpenAsync();
                var command = @"SELECT 
                                c.saldo,
                                c.limite,
                                t.valor,
                                t.tipo_transacao AS tipo,
                                t.descricao,
                                t.realizada_em AS data
                                FROM clientes c
                                LEFT JOIN 
                                transacoes t ON c.id = t.id_cliente
                                WHERE c.id = $1
                                ORDER BY t.realizada_em DESC LIMIT 10";
                using (var cmd = new NpgsqlCommand(command, connection))
                {
                    cmd.Parameters.AddWithValue("$1", id);
                    await cmd.PrepareAsync().ConfigureAwait(false);
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
                    var js = new
                    {
                        saldo = new
                        {
                            total = balance,
                            limite = limit,
                            ultimas_transacoes = list
                        }
                    };
                    return Results.Ok(js);
                }
            }
            //}
        }
    }
}

