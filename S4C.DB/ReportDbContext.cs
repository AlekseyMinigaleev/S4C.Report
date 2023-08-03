using Microsoft.EntityFrameworkCore;
using S4C.DB.Models;

namespace S4C.DB
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options):base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public DbSet<GameModel> GameModels { get; set; }

        public DbSet<GamesStatisticModel> GamesStatisticModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportDbContext).Assembly);
        }
    }
}
