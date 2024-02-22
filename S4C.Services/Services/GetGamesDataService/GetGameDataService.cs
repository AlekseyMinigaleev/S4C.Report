using C4S.Services.Services.GetGamesDataService.Helpers;
using C4S.Services.Services.GetGamesDataService.Models;
using C4S.Services.Services.GetGamesDataService.RequestMethodDictionaries;
using C4S.Shared.Logger;
using C4S.Shared.Models;

namespace C4S.Services.Services.GetGamesDataService
{
    /// <inheritdoc cref="IGetGameDataService"/>
    public class GetGameDataService : IGetGameDataService
    {
        private readonly GetAppIdHelper _getAppIdHelper;
        private readonly GetPublicGameDataHelper _getPublicGameDataHelper;
        private readonly GetPrivateGameDataHelper _getPrivateGameDataHelper;

        public GetGameDataService(
            GetAppIdHelper getAppIdHelper,
            GetPublicGameDataHelper getPublicGameDataHelper,
            GetPrivateGameDataHelper getPrivateGameDataHelper)
        {
            _getAppIdHelper = getAppIdHelper;
            _getPublicGameDataHelper = getPublicGameDataHelper;
            _getPrivateGameDataHelper = getPrivateGameDataHelper;
        }

        /// <inheritdoc/>
        public async Task<PrivateGameData> GetPrivateGameDataAsync(
            int pageId,
            string authorization,
            DateTimeRange period,
            CancellationToken cancellationToken)
        {
            var privateGameData = await _getPrivateGameDataHelper
                .GetPrivateGameDataAsync(
                    pageId: pageId,
                    authorization: authorization,
                    period: period,
                    cancellationToken: cancellationToken);

            return privateGameData;
        }

        /// <inheritdoc/>
        public async Task<PublicGameData[]> GetPublicGameDataAsync(
            string developerPageUrl,
            BaseLogger logger,
            CancellationToken cancellationToken)
        {
            var appIds = await _getAppIdHelper.GetAppIdsAsync(
                developerPageUrl,
                logger,
                cancellationToken);

            var publicGameData = await _getPublicGameDataHelper
                .GetGamesInfoAsync(
                appIds,
                logger,
                cancellationToken);

            return publicGameData;
        }
    }
}