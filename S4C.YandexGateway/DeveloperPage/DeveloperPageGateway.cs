using C4S.Helpers.Logger;
using Newtonsoft.Json.Linq;
using S4C.YandexGateway.DeveloperPageGateway.Exceptions;
using S4C.YandexGateway.DeveloperPageGateway.Models;

namespace S4C.YandexGateway.DeveloperPageGateway
{
    /// <inheritdoc cref="IDeveloperPageGetaway"/>
    public class DeveloperPageGateway : IDeveloperPageGetaway
    {
        /*TODO: хардкод*/
        public readonly string DeveloperPageUrl = "https://yandex.ru/games/developer?name=C4S.SHA";

        private readonly IHttpClientFactory _httpClientFactory;

        public DeveloperPageGateway(
            IHttpClientFactory httpClient)
        {
            _httpClientFactory = httpClient;
        }

        /// <inheritdoc/>
        public async Task<GameInfo[]> GetGameInfoAsync(
            int[] gameIds,
            BaseLogger logger,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation($"Составление запроса на сервер Яндекс");
            var httpResponseMessage = await SendRequestAsync(() =>
                HttpRequestMethodDitctionary.GetGamesInfo(gameIds, "long"),
                cancellationToken);
            logger.LogSuccess($"Ответ от Яндекса успешно получен");

            logger.LogInformation($"Начало обработки ответа");
            var gameViewModels = await DeserializeObjectsAsync(
                httpResponseMessage,
                cancellationToken);
            logger.LogSuccess($"Ответ успешно обработан");

            return gameViewModels;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(
            Func<HttpRequestMessage> createRequest,
            CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient();

            var request = createRequest();
            var response = await client
                .SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        private async Task<GameInfo[]> DeserializeObjectsAsync(
            HttpResponseMessage httpResponseMessage,
            CancellationToken cancellationToken)
        {
            var jsonString = await httpResponseMessage.Content
                .ReadAsStringAsync(cancellationToken);
            var gamesJToken = GetGamesJToken(jsonString);

            var results = new GameInfo[gamesJToken.Count];
            for (int i = 0; i < gamesJToken.Count; i++)
            {
                var title = GetValue<string>("title", gamesJToken[i], jsonString);
                var appId = GetValue<int>("appID", gamesJToken[i], jsonString);
                var firstPublished = GetValue<int>("firstPublished", gamesJToken[i], jsonString);
                var rating = GetValue<double>("rating", gamesJToken[i], jsonString);
                var playersCount = GetValue<int>("playersCount", gamesJToken[i], jsonString);
                var categoriesNames = GetValue<string[]>("categoriesNames", gamesJToken[i], jsonString);

                var gameDataViewModel = new GameInfo(
                    title: title,
                    appId: appId,
                    firstPublished: firstPublished,
                    rating: rating,
                    playersCount: playersCount,
                    categoriesNames: categoriesNames);

                results[i] = gameDataViewModel;
            }

            return results;
        }

        private static JArray GetGamesJToken(string jsonString)
        {
            var responseJObject = JObject.Parse(jsonString);
            var gamesJToken = responseJObject["games"] as JArray
                ?? throw new InvalidContractException(jsonString, "games");

            return gamesJToken;
        }

        private static T GetValue<T>(string key, JToken jToken, string jsonString)
        {
            T value;
            if (typeof(T).IsArray)
                value = jToken[key]!.ToObject<T>()!;
            else
                value = jToken[key]!.Value<T>()!;
            var result = value ?? throw new InvalidContractException(jsonString, key);
            return result;
        }
    }
}