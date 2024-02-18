using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using C4S.Helpers.Logger;
using S4C.YandexGateway.DeveloperPage.Exceptions;

namespace S4C.YandexGateway.DeveloperPage
{
    /// <inheritdoc cref="IDeveloperPageParser"/>
    public class DeveloperPageParser : IDeveloperPageParser
    {
        private readonly IBrowsingContext _browsingContext;
        private string _developerPageUrl;

        public DeveloperPageParser(
            IBrowsingContext browsingContext)
        {
            _browsingContext = browsingContext;
        }

        /// <inheritdoc/>
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
                ?? throw new EmptyDeveloperPageException(_developerPageUrl);

            var path = gameUrlElement!.PathName;

            var gameIdString = GetIdAsString(path);

            var tryParseResult = int.TryParse(gameIdString, out var gameId);

            if (!tryParseResult)
                throw new InvalidGameIdException(gameIdString);

            return gameId;
        }

        private async Task<IHtmlCollection<IElement>> GetGamesAsHtmlElementsAsync(
            CancellationToken cancellationToken = default)
        {
            var document = await _browsingContext
                .OpenAsync(_developerPageUrl, cancellationToken);

            var gridList = document
                .QuerySelector(".grid-list")
                ?? throw new EmptyDeveloperPageException(_developerPageUrl); /*TODO: проверить*/

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