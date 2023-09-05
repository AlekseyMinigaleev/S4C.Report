using C4S.DB.Models;
using C4S.DB.Models.Hangfire;
using Microsoft.EntityFrameworkCore;

namespace C4S.DB
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
        { }

        /// <summary>
        /// Таблица C4S пользователя
        /// </summary>
        public DbSet<UserModel> Users { get; set; }

        /// <summary>
        /// Таблица аккаунта Яндекс игр
        /// </summary>
        public DbSet<YandexGamesAccountModel> YandexGamesAccounts { get; set; }

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