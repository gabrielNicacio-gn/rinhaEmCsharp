using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Models;

namespace src.Repository
{
    public class AppDb : DbContext
    {
        public DbSet<Transactions> Transactions {get;set;}
        public DbSet<Client> Clients {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("DataBase: db");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Cliente>()
            //.HasKey(c=>c.Id);
            modelBuilder.Entity<Transactions>()
            .HasKey(c=>c.Id);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}