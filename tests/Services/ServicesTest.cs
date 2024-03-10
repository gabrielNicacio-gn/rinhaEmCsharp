using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Moq;
using src.DTOs;
using src.Models;
using src.Repository;
using src.Services;

namespace tests.Services
{
    public class ServicesTest
    {
        [Fact]
        public async Task TestConcurrencyAccounts()
        {
            uint idClient = 1;
            int limit = 100000;
            int initialBalance = 0;
            int balance;
            int numberTransaction = 10;
            int value = 9670;

            var mock = new Mock<IRepositorie>();
            mock.Setup(m => m.Exist(It.IsAny<uint>()))
            .ReturnsAsync(true);
            mock.Setup(m => m.Search(idClient)).ReturnsAsync(new Client(idClient, limit, balance = 0 - value * numberTransaction));

            var create = new ServiceClient(mock.Object);

            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await create.PerformTransaction(1, new RequestTransaction(value, 'd', "Compra"));
                }));
            }
            await Task.WhenAll(tasks);

            var client = await mock.Object.Search(idClient);
            var expectedBalance = initialBalance - value * numberTransaction;
            Assert.True(client.Balance > -limit);
            Assert.Equal(expectedBalance, client.Balance);
            Console.Write(expectedBalance + "=" + client.Balance);
        }
    }
}