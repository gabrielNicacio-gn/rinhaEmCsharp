using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using src.DTOs;
using src.Services;

namespace src.Endpoints
{
    public static class RoutesClient
    {
        public static void MapStart (this WebApplication app)
        {
            var endpoints = app.MapGroup("/clientes");

        } 
    }
}