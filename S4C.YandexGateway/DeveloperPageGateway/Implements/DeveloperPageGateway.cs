using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using S4C.YandexGateway.DeveloperPageGateway.Exceptions;
using S4C.YandexGateway.DeveloperPageGateway.Interfaces;
using S4C.YandexGateway.DeveloperPageGateway.Models;
using System.Text;

namespace S4C.YandexGateway.DeveloperPageGateway.Implements
{
    public class DeveloperPageGateway : IDeveloperPageGetaway
    {
        /*TODO: хардкод*/
        public readonly string DeveloperPageUrl = "https://yandex.ru/games/developer?name=C4S.SHA";

        /*TODO: хардкод*/
        public readonly string GetGameInfoUrl = "https://yandex.ru/games/_crpd/2fxg324u1/0463a9o-i/WmeCEDy-VUd2S-vlZ9bXdhXct8xMhg5RScdAENGPZD0MTYp-CnAWBj63mxRiHCIVXFZeDaAw-_pk77fa1o_b6g4Xg239frXHxqdDTPsCJvJt3LvIFdsyWTgHsRSlg2gFVAn_fn8Fai4yy38QRgReUCh-OlGBcNfvWLe74sbqz5Zec-MdKb4RC2o7w6B3xtYebdzT9AWKalVo_1w4uaM0ZTRFyxZfGXY2fqdDaGoYZhAYXmJlpUjnxzjnm7qCgo-iAmfXZTHqURtCO-uoV9q6KgHwv7hZsgJROONgJLXfBAl0VbtaeyV2LmKrXxBiECIYEHJSPa0Ip_N4z8-CmsLHkg5vx31Bsh0LWj-buBuKzm45jOv8PZoyHTjHDAytsxw9IC2zIgc1ek4ugztIQgBeUEACDlHlYIuLZN_7_sb6s_5uG-M1WcYtV2Ybv7B31qZSAZiv_BH6MglQu3AklesQUVA1-3IDMQJM";

        private readonly IBrowsingContext _browsingContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public DeveloperPageGateway(IHttpClientFactory httpClient)
        {
            /*TODO: как это сделать черзе DI?*/
            var config = Configuration.Default.WithDefaultLoader();
            _browsingContext = BrowsingContext.New(config);
            _httpClientFactory = httpClient;
        }

        public async Task<int[]> GetGameIdsAsync()
        {
            var gamesHtmlCollection = await ParseAllGamesAsync(DeveloperPageUrl);

            var gameIds = new List<int>();
            foreach (var gameElement in gamesHtmlCollection)
            {
                var id = GetGameId(gameElement);
                gameIds.Add(id);
            }

            /*TODO: странная запись*/
            return gameIds.ToArray();
        }

        public async Task<GameInfo[]> GetGameInfoAsync(int[] gameIds)
        {
            var payload = GetPayload(gameIds);

            var httpResponseMessage = await SendRequestAsync(payload);

            var gameViewModels = await DeserializeObjectsAsync(httpResponseMessage, gameIds.Length);

            /*TODO: странная запись*/
            return gameViewModels.ToArray();
        }

        private async Task<IHtmlCollection<IElement>> ParseAllGamesAsync(string developerPageUrl)
        {
            var document = await _browsingContext
                .OpenAsync(developerPageUrl);

            var gridList = document
                .QuerySelector(".grid-list")
                ?? throw new EmptyDeveloperPageException(developerPageUrl);

            var children = gridList.Children;
            return children;
        }

        private static int GetGameId(IElement element)
        {
            var gameUrlElement = element
                .QuerySelector(".game-url") as IHtmlAnchorElement;

            ThrowIfNull(gameUrlElement, "У элемента отсутствует ссылка на игру");

            var path = gameUrlElement!.PathName;

            var gameIdString = GetIdAsString(path);

            var tryParseResult = int.TryParse(gameIdString, out var gameId);

            if(!tryParseResult)
            {
                throw new InvalidGameIdException(gameIdString);
            }

            return gameId;
        }

        private static string GetIdAsString(string path)
        {
            var lastIndex = path.LastIndexOf("/");
            var gameId = path[(lastIndex + 1)..];
            return gameId;
        }

        private static void ThrowIfNull(object? @object, string message)
        {
            var condition = @object is null;
            ThrowIf(condition, message);
        }

        private static void ThrowIf(bool condition, string message)
        {
            if (condition)
                throw new Exception(message);
        }

        private static StringContent GetPayload(int[] gameIds)
        {
            var requestData = new
            {
                appIDs = gameIds,
                format = "long"
            };

            string jsonPayload = JsonConvert.SerializeObject(requestData);

            var payload = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            return payload;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpContent httpContent)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(GetGameInfoUrl, httpContent);
            response.EnsureSuccessStatusCode();
            return response;
        }

        private async Task<List<GameInfo>> DeserializeObjectsAsync(HttpResponseMessage httpResponseMessage, int gameCount)
        {
            var (gamesJToken, jsonString) = await GetGamesJTokenAndJsonStringAsync(httpResponseMessage);

            var results = new List<GameInfo>();
            for (int i = 0; i < gameCount; i++)
            {
                var gameJToken = gamesJToken[i]
                    ?? throw new InvalidContractException(jsonString, "Array is empty");

                var appId = (int?)gameJToken["appID"]
                    ?? throw new InvalidContractException(jsonString, "appID");

                var name = (string?)gameJToken["title"]
                    ?? throw new InvalidContractException(jsonString, "title");

                var firstPublished = (int?)gameJToken["firstPublished"]
                    ?? throw new InvalidContractException(jsonString, "firstPublished");

                var rating = (double?)gameJToken["rating"]
                    ?? throw new InvalidContractException(jsonString, "rating");

                var playersCount = (int?)gameJToken["playersCount"]
                    ?? throw new InvalidContractException(jsonString, "playersCount");

                var categoriesNames = gameJToken["categoriesNames"]!.ToObject<string[]>()
                    ?? throw new InvalidContractException(jsonString, "categoriesNames");

                var gameDataViewModel = new GameInfo(
                    name: name,
                    appId: appId,
                    firstPublished: firstPublished,
                    rating: rating,
                    playersCount: playersCount,
                    categoriesNames: categoriesNames);

                results.Add(gameDataViewModel);
            }

            return results;
        }

        private async Task<(JToken, string)> GetGamesJTokenAndJsonStringAsync(HttpResponseMessage httpResponseMessage)
        {
            var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();

            var jObject = JsonConvert
                .DeserializeObject<JObject>(jsonString)
                ?? throw new InvalidContractException(jsonString);

            var gamesJToken = jObject["games"]
                ?? throw new InvalidContractException(jsonString, "games");
            return (gamesJToken, jsonString);
        }
    }
}