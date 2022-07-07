using Microsoft.EntityFrameworkCore;

namespace TibiaCharFinder.Entities
{
    public class EnemyCharFinderDbContext : DbContext
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=TibiaCharFinderDb; Trusted_Connection=True";

        public DbSet<World> Worlds { get; set; }
        public DbSet<Scan> Scans { get; set; }
        public DbSet<ScanWorld> ScanWorlds { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Correlation> Correlations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<World>()
                .Property(p => p.Name)
                .IsRequired();
            modelBuilder.Entity<World>()
                .Property(p => p.Url)
                .IsRequired(); 

            modelBuilder.Entity<Scan>()
                .Property(p => p.CharactersOnline)
                .IsRequired();
            modelBuilder.Entity<Scan>()
                .Property(p => p.ScanCreateDateTime)
                .IsRequired();
            
            modelBuilder.Entity<Character>()
                .Property(p => p.Name)
                .IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
