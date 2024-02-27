using C4S.DB;
using C4S.DB.Models;
using C4S.DB.ValueObjects;
using C4S.Services.Services.GetGamesDataService;
using C4S.Services.Services.GetGamesDataService.Models;
using C4S.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace C4S.Services.Services.GameSyncService.Helpers
{
    public class GameModelEnricher
    {
        private readonly ReportDbContext _dbContext;
        private readonly IGetGameDataService _getGameDataService;
        private UserModel _user;

        public GameModelEnricher(
            ReportDbContext dbContext,
            IGetGameDataService gameDataService)
        {
            _dbContext = dbContext;
            _getGameDataService = gameDataService;
        }

        public async Task<GameModel> EnrichGameModelsAsync(
            GameModel gameModelToEnrich,
            PublicGameData publicGameData,
            CancellationToken cancellationToken)
        {
            _user = gameModelToEnrich.User;
            var authorizationToken = _user.RsyaAuthorizationToken;
            var existGameModel = _dbContext.Games
                .Include(x => x.GameStatistics)
                .Where(x => x.UserId == _user.Id)
                .SingleOrDefault(x => x.AppId == gameModelToEnrich.AppId);

            var categories = await GetCategories(publicGameData, cancellationToken);
            var rating = GetRating(existGameModel, publicGameData);
            ValueWithProgress<double>? cashIncome = null;
            if (authorizationToken is not null)
                cashIncome = await GetCashIncomeAsync(gameModelToEnrich, existGameModel, cancellationToken);

            gameModelToEnrich.AddCategories(categories);
            gameModelToEnrich.GameStatistics.First().Rating = rating;
            gameModelToEnrich.GameStatistics.First().CashIncome = cashIncome;

            return gameModelToEnrich;
        }

        private async Task<IEnumerable<CategoryModel>> GetCategories(
            PublicGameData publicGameData,
            CancellationToken cancellationToken)
        {
            var categories = await _dbContext.Categories
                .Where(x => publicGameData.CategoriesNames.Contains(x.Name))
                .ToListAsync(cancellationToken);

            return categories;
        }

        private async Task<ValueWithProgress<double>?> GetCashIncomeAsync(
            GameModel gameModelToEnrich,
            GameModel? existGameModel,
            CancellationToken cancellationToken)
        {
            if (existGameModel is null || !existGameModel.PageId.HasValue)
                return null;

            var startDate = gameModelToEnrich.PublicationDate;
            var endDate = DateTime.Now;
            var period = new DateTimeRange(startDate, endDate);

            var actualCashIncome = (await _getGameDataService
                .GetPrivateGameDataAsync(
                    pageId: existGameModel.PageId.Value,
                    authorization: _user.RsyaAuthorizationToken!,
                    period: period,
                    cancellationToken: cancellationToken))
                .CashIncome;

            var lastSynchroGameStatisticWithCashIncome = existGameModel.GameStatistics
                .Where(x => x.CashIncome is not null)
                .OrderByDescending(x => x.LastSynchroDate)
                .FirstOrDefault();

            ValueWithProgress<double>? cashIncome;

            if (lastSynchroGameStatisticWithCashIncome is null)
            {
                if (actualCashIncome.HasValue)
                {
                    cashIncome = new ValueWithProgress<double>(
                        actualCashIncome.Value,
                        actualCashIncome.Value);
                }
                else
                {
                    cashIncome = null;
                }
            }
            else
            {
                if (actualCashIncome.HasValue)
                {
                    cashIncome = new ValueWithProgress<double>(
                        actualCashIncome.Value,
                        actualCashIncome.Value - lastSynchroGameStatisticWithCashIncome.CashIncome!.ActualValue);
                }
                else
                {
                    cashIncome = new ValueWithProgress<double>(
                            0,
                            0 - lastSynchroGameStatisticWithCashIncome.CashIncome!.ActualValue);
                }
            }

            return cashIncome;
        }

        private ValueWithProgress<int>? GetRating(
            GameModel? existGameModel,
            PublicGameData publicGameData)
        {
            if (!publicGameData.Rating.HasValue)
                return null;

            ValueWithProgress<int>? rating;

            if (existGameModel is not null)
            {
                var lastSynchroGameStatisticWithRating = existGameModel.GameStatistics
                    .Where(x => x.Rating is not null)
                    .OrderByDescending(x => x.LastSynchroDate)
                    .FirstOrDefault();

                if (lastSynchroGameStatisticWithRating is not null)
                {
                    rating = new ValueWithProgress<int>(
                        publicGameData.Rating.Value,
                        publicGameData.Rating.Value - lastSynchroGameStatisticWithRating.Rating!.ActualValue);
                }
                else
                {
                    rating = new ValueWithProgress<int>(
                       publicGameData.Rating.Value,
                       publicGameData.Rating.Value);
                }
            }
            else
            {
                rating = new ValueWithProgress<int>(
                    publicGameData.Rating.Value,
                    publicGameData.Rating.Value);
            }

            return rating;
        }
    }
}