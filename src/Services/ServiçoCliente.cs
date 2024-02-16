using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using src.DTOs;
using src.Models;

namespace src.Services
{
    public class ServiçoCliente
    {
        public ConcurrentQueue<Transacao> transacaos; 
        public async Task<IResult> RealizarTransacao(int id, RequisicaoTransacao requisicao)
        {
           return Results.Ok();
        }

        public async Task<IResult> ConsultarExtrato()
        {
            return Results.Ok();
        } 
    }
}