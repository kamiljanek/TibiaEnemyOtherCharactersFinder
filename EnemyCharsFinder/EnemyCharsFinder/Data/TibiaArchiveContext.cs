using EnemyCharsFinder.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyCharsFinder.Data
{
    public class TibiaArchiveContext : DbContext
    {
        public DbSet<Url> Urls { get; set; }
        public DbSet<ScrapSesion> ScrapSesions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=TibiaArchive; Integrated Security=True");
            //optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=TibiaArchive; Integrated Security=True; Connect Timeout=30; Encrypt=False; TrustServerCertificate=False; ApplicationIntent=ReadWrite; MultiSubnetFailover=False");
        }

    }
}
