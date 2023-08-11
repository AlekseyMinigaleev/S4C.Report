using Microsoft.EntityFrameworkCore;
using S4C.DB.Models;
using S4C.DB.Models.Hangfire;

namespace S4C.DB
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options):base(options)
        { }

        public DbSet<GameModel> GameModels { get; set; }

        public DbSet<GamesStatisticModel> GamesStatisticModels { get; set; }

        public DbSet<HangfireJobConfigurationModel> HangfireConfigurationModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportDbContext).Assembly);
        }
    }
}
