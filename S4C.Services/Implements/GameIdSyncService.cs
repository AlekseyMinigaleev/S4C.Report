using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.HangfireHelpers;
using C4S.Services.Interfaces;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using S4C.YandexGateway.DeveloperPageGateway;
using S4C.YandexGateway.DeveloperPageGateway.Exceptions;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IGameIdSyncService"/>
    public class GameIdSyncService : IGameIdSyncService
    {
        private readonly IDeveloperPageGetaway _developerPageGetaway;
        private readonly ReportDbContext _dbContext;
        private  IHangfireLogger _hangfireLogger;

        public GameIdSyncService(
            ReportDbContext dbContext,
            IDeveloperPageGetaway developerPageGetaway)
        {
            _dbContext = dbContext;
            _developerPageGetaway = developerPageGetaway;
        }

        /// <inheritdoc/>
        public async Task SyncAllGameIdAsync(
            PerformContext hangfireContext,
            CancellationToken cancellationToken = default)
        {
            _hangfireLogger = new HangfireLogger(hangfireContext);

            var finalLogMessage = "Процесс успешно завершен.";
            var errorLogMessage = "Процесс завершен c ошибкой: ";
            var warningLogMessage = "Процесс завершен: ";
            var logLevel = HangfireLogLevel.Success;
            try
            {
                _hangfireLogger.LogInformation($"Запущен процесс парсинга id игр:");
                await RunAsync(cancellationToken);
            }
            catch (EmptyDeveloperPageException e)
            {
                finalLogMessage = $"{warningLogMessage}{e.Message}";
                logLevel = HangfireLogLevel.Warning;
            }
            catch (InvalidGameIdException e)
            {
                finalLogMessage = $"{errorLogMessage}{e.Message}";
                logLevel = HangfireLogLevel.Error;
            }
            catch (Exception e)
            {
                finalLogMessage = $"{errorLogMessage}{e.Message}";
                logLevel = HangfireLogLevel.Error;
            }
            finally
            {
                _hangfireLogger.Log(finalLogMessage, logLevel);
            }
        }

        private async Task RunAsync(
            CancellationToken cancellationToken)
        {
            /*TODO: поправить логи*/
            _hangfireLogger.LogInformation($"Получение id всех игр.");
            var gameIds = await _developerPageGetaway
                .GetGameIdsAsync(cancellationToken);
            _hangfireLogger.LogSuccess($"Успешно полученных id: {gameIds.Length}.");

            /*TODO: поправить логи*/
            _hangfireLogger.LogInformation($"Начало обработки всех id:");
            await ProcessingIncomingGameIdsAsync(gameIds, cancellationToken);
            _hangfireLogger.LogSuccess($"Все id обработаны.");
        }

        private async Task ProcessingIncomingGameIdsAsync(
            int[] gameIds,
            CancellationToken cancellationToken)
        {
            var countOfMissingGames = 0;
            foreach (var gameId in gameIds)
            {
                var increment = await ProcessingIncomingIdAsync(gameId, cancellationToken);
                countOfMissingGames += increment;
            }
            _hangfireLogger.LogInformation($"Начло добавления {countOfMissingGames}, записей в базу данных:");
            await _dbContext.SaveChangesAsync(cancellationToken);
            _hangfireLogger.LogSuccess($"Все записи успешно добавлены");
        }

        // этот метод возвращает инкремент для переменной countOfMissingGame
        private async Task<int> ProcessingIncomingIdAsync(
            int gameId,
            CancellationToken cancellationToken)
        {
            var existingGameIds = _dbContext.GameModels
                .Select(x => x.Id);

            var isExistingGame = await existingGameIds
                .ContainsAsync(gameId, cancellationToken);

            if (!isExistingGame)
            {
                _hangfireLogger.LogInformation($"[{gameId}] id игры не содержится в базе данных.");
                var gameModel = new GameModel(gameId);
                await _dbContext.GameModels
                    .AddAsync(gameModel, cancellationToken);
                _hangfireLogger.LogInformation($"[{gameId}] id игры помечено на добавление в базу данных.");

                // если игра не содержится в бд, увеличиваем countOfMissingGame
                return 1;
            }
            else
            {
                _hangfireLogger.LogInformation($"[{gameId}] id игры уже содержится в базе данных.");

                // если игра содержится в бд, не увеличиваем countOfMissingGame
                return 0;
            }
        }
    }
}