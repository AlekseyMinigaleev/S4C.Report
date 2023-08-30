using Newtonsoft.Json.Linq;
using S4C.YandexGateway.DeveloperPageGateway.Exceptions;
using S4C.YandexGateway.DeveloperPageGateway.Models;

namespace S4C.YandexGateway.DeveloperPageGateway
{
    public class DeveloperPageGateway : IDeveloperPageGetaway
    {
        /*TODO: хардкод*/
        public readonly string DeveloperPageUrl = "https://yandex.ru/games/developer?name=C4S.SHA";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DeveloperPageParser _developerPageParser; /*TODO: не понимаю нужен тут интерфейс или нет*/

        public DeveloperPageGateway(
            IHttpClientFactory httpClient,
            DeveloperPageParser developerPageParser)
        {
            _httpClientFactory = httpClient;
            _developerPageParser = developerPageParser;
        }

        public async Task<int[]> GetGameIdsAsync()
        {
            var gameIds = await _developerPageParser.GetAllGameidAsync(DeveloperPageUrl);

            return gameIds;
        }

        public async Task<GameInfo[]> GetGameInfoAsync(int[] gameIds)
        {
            var httpResponseMessage = await SendRequestAsync(() =>
                HttpRequestMethods.GetGameInfo(gameIds, "long"));

            var gameViewModels = await DeserializeObjectsAsync(httpResponseMessage);

            return gameViewModels;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(Func<HttpRequestMessage> createRequest)
        {
            var client = _httpClientFactory.CreateClient();

            var request = createRequest();
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return response;
        }

        private async Task<GameInfo[]> DeserializeObjectsAsync(HttpResponseMessage httpResponseMessage)
        {
            var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
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