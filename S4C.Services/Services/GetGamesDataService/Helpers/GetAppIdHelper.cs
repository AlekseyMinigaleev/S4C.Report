using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using C4S.Helpers.Logger;
using C4S.Services.Services.GetGamesDataService.Models;

namespace C4S.Services.Services.GetGamesDataService.Helpers
{
    /// <summary>
    /// Вспомогательный класс для получения идентификаторов приложений (<see cref="PublicGameData.AppId"/>) игр из веб-страницы разработчика.
    /// </summary>
    public class GetAppIdHelper
    {
        private readonly IBrowsingContext _browsingContext;
        private string _developerPageUrl;

        public GetAppIdHelper(
            IBrowsingContext browsingContext)
        {
            _browsingContext = browsingContext;
        }

        /// <summary>
        /// Получает идентификаторы приложений (<see cref="PublicGameData.AppId"/>) игр из указанной веб-страницы разработчика.
        /// </summary>
        /// <param name="developerPageUrl">URL веб-страницы разработчика.</param>
        /// <param name="logger">Экземпляр логгера для записи информационных сообщений.</param>
        /// <param name="cancellationToken">Токен отмены задачи.</param>
        /// <returns>Массив целочисленных значений, представляющих <see cref="PublicGameData.AppId"/> игр.</returns>
        public async Task<int[]> GetAppIdsAsync(
            string developerPageUrl,
            BaseLogger logger,
            CancellationToken cancellationToken = default)
        {
            _developerPageUrl = developerPageUrl;

            logger.LogInformation("Начат процесс получения AppId игр");

            logger.LogInformation("Получение игр как html элементов");
            var gamesHtmlCollection = await GetGamesAsHtmlElementsAsync(
                cancellationToken);
            logger.LogSuccess($"Успешно получено {gamesHtmlCollection.Count()} элементов");

            logger.LogInformation("Получения id из html элементов");
            var gameIds = new int[gamesHtmlCollection.Length];
            for (int i = 0; i < gamesHtmlCollection.Length; i++)
            {
                var id = GetGameId(gamesHtmlCollection[i]);
                gameIds[i] = id;
            }
            logger.LogSuccess($"Успешно получено {gameIds.Length} id");

            logger.LogSuccess("Процесс получения AppId игр успешно завершен");
            return gameIds;
        }

        private int GetGameId(IElement element)
        {
            var gameUrlElement = element
                .QuerySelector(".game-url") as IHtmlAnchorElement
                ?? throw new Exception($"На странице {_developerPageUrl} нет игр");
            /*TODO: сделать общую ошибку для случая когда с парсинга приходит null*/

            var path = gameUrlElement!.PathName;

            var gameIdString = GetIdAsString(path);

            var tryParseResult = int.TryParse(gameIdString, out var gameId);

            if (!tryParseResult)
                throw new Exception($"не удалось преобразовать id - {gameIdString} в int");
            /*TODO: сделать общую ошибку для случая когда с парсинга приходит null*/

            return gameId;
        }

        private async Task<IHtmlCollection<IElement>> GetGamesAsHtmlElementsAsync(
            CancellationToken cancellationToken = default)
        {
            var document = await _browsingContext
                .OpenAsync(_developerPageUrl, cancellationToken);

            var gridList = document
                .QuerySelector(".grid-list")
                ?? throw new Exception($"На странице {_developerPageUrl} нет игр"); /*TODO: Exception*/
            /*TODO: сделать общую ошибку для случая когда с парсинга приходит null*/

            var children = gridList.Children;

            return children;
        }

        private static string GetIdAsString(string path)
        {
            var lastIndex = path.LastIndexOf("/");
            var gameId = path[(lastIndex + 1)..];
            return gameId;
        }
    }
}