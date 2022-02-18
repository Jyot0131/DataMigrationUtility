using DataMigrationUtility.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationUtility.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Source> SourceTable { get; set; }
        public DbSet<Destination> DestinationTable { get; set; }
        public DbSet<BatchStats> BatchStats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog=MigrationDatabase");
                //.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();

        }
    }
}
