using Microsoft.EntityFrameworkCore;
using C4S.DB.Models;
using C4S.DB.Models.Hangfire;

namespace C4S.DB
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options):base(options)
        { }

        /// <summary>
        /// Таблица игры
        /// </summary>
        public DbSet<GameModel> GameModels { get; set; }

        /// <summary>
        /// Таблица игровой статистики
        /// </summary>
        public DbSet<GameStatisticModel> GamesStatisticModels { get; set; }

        /// <summary>
        /// Таблица джобы.
        /// </summary>
        public DbSet<HangfireJobConfigurationModel> HangfireConfigurationModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportDbContext).Assembly);
        }
    }
}
