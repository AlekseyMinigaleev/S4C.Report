using C4S.Helpers.Extensions;
using C4S.Helpers.Logger;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using S4C.Helpers;
using S4C.YandexGateway.DeveloperPage.Enums;
using S4C.YandexGateway.DeveloperPage.Models;

namespace S4C.YandexGateway.DeveloperPage
{
    /// <inheritdoc cref="IDeveloperPageGetaway"/>
    public class DeveloperPageGateway : IDeveloperPageGetaway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _yandexGamesRequestUrl;

        public DeveloperPageGateway(
            IHttpClientFactory httpClient,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClient;
            _yandexGamesRequestUrl = configuration["YandexGamesRequestUrl"]!;
            /*TODO: ??????*/
            ArgumentException.ThrowIfNullOrEmpty("в файле appsetting.json не указана или указана неверно ссылка на запрос по Яндекс играм");
        }

        /// <inheritdoc/>
        public async Task<GameInfoModel[]> GetGameInfoAsync(
            int[] gameIds,
            BaseLogger logger,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation($"Составление запроса на сервер Яндекс");
            var createRequest = () =>
            HttpRequestMethodDitctionary.GetGamesInfo(
                    _yandexGamesRequestUrl,
                    gameIds,
                    RequestFormat.Long);

            var httpResponseMessage = await HttpUtils.SendRequestAsync(
                createRequest,
                _httpClientFactory,
                cancellationToken);
            logger.LogSuccess($"Ответ от Яндекса успешно получен");

            logger.LogInformation($"Начало обработки ответа");
            var gameInfoModel = await DeserializeObjectsAsync(
                httpResponseMessage,
                cancellationToken);
            logger.LogSuccess($"Ответ успешно обработан");

            return gameInfoModel;
        }

        private async Task<GameInfoModel[]> DeserializeObjectsAsync(
            HttpResponseMessage httpResponseMessage,
            CancellationToken cancellationToken)
        {
            var jsonString = await httpResponseMessage.Content
                .ReadAsStringAsync(cancellationToken);

            var jObject = JObject.Parse(jsonString);
            var gamesJToken = jObject.GetValue<JArray>("games");

            var results = new GameInfoModel[gamesJToken.Count];
            for (int i = 0; i < gamesJToken.Count; i++)
            {
                var title = gamesJToken[i].GetValue<string>("title");
                var appId = gamesJToken[i].GetValue<int>("appID");
                var firstPublished = gamesJToken[i].GetValue<int>("firstPublished");
                var rating = gamesJToken[i].GetValue<double>("rating");
                var playersCount = gamesJToken[i].GetValue<int>("playersCount");
                var categoriesNames = gamesJToken[i].GetValue<string[]>("categoriesNames");

                var gameInfo = new GameInfoModel(
                    title: title,
                    appId: appId,
                    firstPublished: firstPublished,
                    rating: rating,
                    playersCount: playersCount,
                    categoriesNames: categoriesNames);

                results[i] = gameInfo;
            }

            return results;
        }
    }
}