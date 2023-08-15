using Microsoft.EntityFrameworkCore;
using C4S.DB.Models;
using C4S.DB.Models.Hangfire;

namespace C4S.DB
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
