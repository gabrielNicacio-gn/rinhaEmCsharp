using Microsoft.AspNetCore.Mvc;
using src.Services;
using src.DTOs;
using src.Repository;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRepositorie, Repositorie>();
//builder.Services.AddScoped<AppDb>();
builder.Services.AddScoped<ServiceClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapGet("cliente/{id}", async (uint id, [FromServices] ServiceClient _servico) =>
{
    return await _servico.ConsultarExtrato(id);
});

app.MapPost("cliente/{id}/transacao", async (uint id,
                [FromServices] ServiceClient _service,
                [FromBody] RequestTransaction request) =>
{
    return await _service.PerformTransaction(id, request);
});
app.Run();

