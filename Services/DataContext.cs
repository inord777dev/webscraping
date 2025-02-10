using MauiScrap;
using MauiScrap.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScrap.Services
{
    public class DataContext : DbContext
    {
        private readonly int PRAGMA_USER_VERSION = 1;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<PriceChanges> PriceChanges { get; set; } = null!;

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            if (!Database.EnsureCreated())
            {
                long user_version = Database.SqlQueryRaw<long>("PRAGMA user_version")
                                        .AsEnumerable().FirstOrDefault();

                while (PRAGMA_USER_VERSION > user_version)
                {
                    try
                    {
                        user_version += 1;
                        switch (user_version)
                        {
                            case 1:
                                UpdateUserVersion_1();
                                break;
                            default:
                                break;
                        }
                        Database.ExecuteSqlRaw($"PRAGMA user_version={user_version}");
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void UpdateUserVersion_1()
        {
            String script = "ALTER TABLE Products ADD Created DATETIME";
            Database.ExecuteSqlRaw(script);

            script = string.Format("UPDATE Products SET Created = '{0:dd.MM.yyyy}'", DateTime.MinValue);
            Database.ExecuteSqlRaw(script);
        }
    }
}
