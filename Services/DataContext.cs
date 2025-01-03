using MauiScrap;
using MauiScrap.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScrap.Services
{
    public class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<PriceChanges> PriceChanges { get; set; } = null!;
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "kufar.db");
        //    optionsBuilder.UseSqlite($"Filename={dbPath}");
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlite("Data Source=kufar.db");
        //}

        public DataContext() 
        {
            Database.EnsureCreated();
        }

        //use for DI!
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
            Database.EnsureCreated();
        }

        //use for migration!
        protected override void OnConfiguring(DbContextOptionsBuilder options)
             => options.UseSqlite();
    }
}
