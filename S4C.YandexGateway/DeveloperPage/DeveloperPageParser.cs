using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using S4C.YandexGateway.DeveloperPageGateway.Exceptions;

namespace S4C.YandexGateway.DeveloperPageGateway
{
    /// <summary>
    /// Класс выполняющий парсинг html документа, страницы разработчика.
    /// </summary>
    public class DeveloperPageParser
    {
        private readonly IBrowsingContext _browsingContext;

        public DeveloperPageParser(IBrowsingContext browsingContext)
        {
            _browsingContext = browsingContext;
        }

        /// <summary>
        /// Возвращает id всех игр со страницы разработчика.
        /// </summary>
        /// <returns>
        ///<see cref="int"/>[] представляющих id всех игр на странице разработчика.
        /// </returns>
        /// <exception cref="EmptyDeveloperPageException"></exception>
        /// <exception cref="InvalidGameIdException"></exception>
        public async Task<int[]> GetAllGameidAsync(
            string developerPageUrl,
            CancellationToken cancellationToken = default)
        {
            var gamesHtmlCollection = await GetGamesAsHtmlElementsAsync(
                developerPageUrl,
                cancellationToken);

            var gameIds = new int[gamesHtmlCollection.Length];

            for (int i = 0; i < gamesHtmlCollection.Length; i++)
            {
                var id = GetGameId(gamesHtmlCollection[i], developerPageUrl);
                gameIds[i] = id;
            }

            return gameIds;
        }

        private static int GetGameId(IElement element, string developerPageUrl)
        {
            var gameUrlElement = element
                .QuerySelector(".game-url") as IHtmlAnchorElement
                ?? throw new EmptyDeveloperPageException(developerPageUrl);

            var path = gameUrlElement!.PathName;

            var gameIdString = GetIdAsString(path);

            var tryParseResult = int.TryParse(gameIdString, out var gameId);

            if (!tryParseResult)
                throw new InvalidGameIdException(gameIdString);

            return gameId;
        }

        private async Task<IHtmlCollection<IElement>> GetGamesAsHtmlElementsAsync(
            string developerPageUrl,
            CancellationToken cancellationToken = default)
        {
            var document = await _browsingContext
                .OpenAsync(developerPageUrl, cancellationToken);

            var gridList = document
                .QuerySelector(".grid-list")
                ?? throw new EmptyDeveloperPageException(developerPageUrl); /*TODO: проверить*/

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