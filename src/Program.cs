using Microsoft.AspNetCore.Mvc;
using src.Services;
using src.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ServiceClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

var routes = app.MapGroup("/clientes");

routes.MapGet("/{id}/extrato", async (int id, [FromServices] ServiceClient _servico) =>
{
    return await _servico.GetExtract(id);
});

routes.MapPost("/{id}/transacoes", async (int id,
                [FromServices] ServiceClient _service,
                [FromBody] RequestTransaction request) =>
{
    return await _service.PerformTransaction(id, request);
});

app.Run();

