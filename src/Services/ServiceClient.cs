
using Dapper;
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
    private string connStr = "Host=db;Port=5432;Database=rinha;Username=admin;Password=123;";
    public async Task<IResult> PerformTransaction(int id, RequestTransaction request)
    {
        if (id < 1 || id > 5)
        {
            return Results.NotFound("Não Existe");
        }
        if (request.Tipo != 'c' && request.Tipo != 'd'
        || request.Descricao.Length > 10
        || request.Valor <= 0
        || !(request.Valor % 1 == 0)
        || string.IsNullOrEmpty(request.Descricao))
        {
            return Results.UnprocessableEntity();
        }
        using (var connection = new NpgsqlConnection(connStr))
        {
            var update = request.Tipo == 'c' ? @"
                                BEGIN;   
                                INSERT INTO transacao (valor,id_cliente,tipo_transacao,descricao,realizada_em) 
                                VALUES (@valor,@id_cliente,@tipo_transacao,@descricao,NOW()); 

                                UPDATE cliente SET saldo = saldo + @valor 
                                WHERE id = @id_cliente 
                                RETURNING saldo,limite;
                                COMMIT;
                                "
                        : @"
                                BEGIN;
                                INSERT INTO transacao (valor,id_cliente,tipo_transacao,descricao,realizada_em) 
                                VALUES (@valor,@id_cliente,@tipo_transacao,@descricao,NOW()); 

                                UPDATE cliente SET saldo = saldo - @valor 
                                WHERE id = @id_cliente 
                                AND saldo - @valor >= - limite
                                RETURNING saldo,limite;
                                COMMIT;
                                ";
            await connection.OpenAsync().ConfigureAwait(false);

            using (var cmd = new NpgsqlCommand(update, connection))
            {
                var transaction = new Transactions(request.Valor, request.Tipo, request.Descricao);
                cmd.Parameters.AddWithValue("@valor", transaction.Value);
                cmd.Parameters.AddWithValue("@id_cliente", id);
                cmd.Parameters.AddWithValue("@tipo_transacao", transaction.Type);
                cmd.Parameters.AddWithValue("@descricao", transaction.Description);

                await cmd.PrepareAsync().ConfigureAwait(false);

                var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
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

    public async Task<IResult> GetExtract(int id)
    {
        if (id < 1 || id > 5)
        {
            return Results.NotFound("Não Existe");
        }

        using (var connection = new NpgsqlConnection(connStr))
        {
            await connection.OpenAsync().ConfigureAwait(false);
            var command = @"
                            BEGIN;
                            SELECT saldo, limite FROM cliente WHERE id = @id;
                            SELECT valor, tipo_transacao, descricao, realizada_em FROM transacao WHERE id_cliente = @id ORDER BY realizada_em DESC LIMIT 10;
                            COMMIT;";

            using (var responses = await connection.QueryMultipleAsync(command, new { id = id }).ConfigureAwait(false))
            {
                var saldo = await responses.ReadFirstAsync<Balance>().ConfigureAwait(false);
                var list_transactions = await responses.ReadAsync<ResponseTransactions>().ConfigureAwait(false);
                var ultimas_transacoes = new List<ResponseTransactions>();
                var js = new
                {
                    saldo = new ExtractResponse(saldo.saldo, saldo.limite, DateTime.UtcNow),
                    ultimas_transacoes = list_transactions
                };
                return Results.Ok(js);
            };
        }
    }
}



