using Microsoft.EntityFrameworkCore;

namespace TibiaCharFinder.Entities
{
    public class EnemyCharFinderDbContext : DbContext
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=TibiaCharFinderDb; Trusted_Connection=True";

        public DbSet<World> Worlds { get; set; }
        public DbSet<Scan> Scans { get; set; }
<<<<<<< HEAD
<<<<<<< HEAD
        public DbSet<WorldScan> WorldScans { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<WorldCorrelation> WorldCorrelations { get; set; }
=======
        public DbSet<ScanWorld> ScanWorlds { get; set; }
        public DbSet<Character> Characters { get; set; }
>>>>>>> 7adfd3e7255e39674243874a51dd79850ec583bd
=======
        public DbSet<ScanWorld> ScanWorlds { get; set; }
        public DbSet<Character> Characters { get; set; }
>>>>>>> 7adfd3e7255e39674243874a51dd79850ec583bd
        public DbSet<Correlation> Correlations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<World>()
                .Property(p => p.Name)
                .IsRequired();
<<<<<<< HEAD
<<<<<<< HEAD
      
            modelBuilder.Entity<Scan>()
                .Property(p => p.CharactersOnline)
                .IsRequired();
            modelBuilder.Entity<Scan>()
                .Property(p => p.ScanCreateDateTime)
                .IsRequired();

            modelBuilder.Entity<WorldScan>()
                .Property(p => p.CharactersOnline)
                .IsRequired();
            modelBuilder.Entity<WorldScan>()
=======
=======
>>>>>>> 7adfd3e7255e39674243874a51dd79850ec583bd
            modelBuilder.Entity<World>()
                .Property(p => p.Url)
                .IsRequired(); 

            modelBuilder.Entity<Scan>()
                .Property(p => p.CharactersOnline)
                .IsRequired();
            modelBuilder.Entity<Scan>()
<<<<<<< HEAD
>>>>>>> 7adfd3e7255e39674243874a51dd79850ec583bd
=======
>>>>>>> 7adfd3e7255e39674243874a51dd79850ec583bd
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
