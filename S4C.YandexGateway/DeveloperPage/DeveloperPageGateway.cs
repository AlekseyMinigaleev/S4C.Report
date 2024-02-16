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
        private string _yandexGetGameRequestURL;
        private const string PreviewFormat = "pjpg350x209";

        public DeveloperPageGateway(
            IHttpClientFactory httpClient,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClient;
            _yandexGetGameRequestURL = configuration["YandexGetGame"]!;

            ArgumentException.ThrowIfNullOrEmpty(
                _yandexGetGameRequestURL,
                "в файле appsetting.json не указана или указана неверно ссылка на запрос по Яндекс играм");
        }

        /// <inheritdoc/>
        public async Task<GameInfoModel[]> GetGamesInfoAsync(
            int[] appIds,
            BaseLogger logger,
            CancellationToken cancellationToken = default)
        {
            var result = new List<GameInfoModel>();
            foreach (var appId in appIds)
            {
                var loggerPrefix = $"[{appId}]";
                logger.LogInformation($"{loggerPrefix} Составление запроса на сервер Яндекс");
                HttpRequestMessage createRequest() =>
                    HttpRequestMethodDitctionary.GetGamesInfo(
                        _yandexGetGameRequestURL,
                        appId,
                        RequestFormat.Long);

                var httpResponseMessage = await HttpUtils.SendRequestAsync(
                    createRequest,
                    _httpClientFactory,
                    cancellationToken);
                logger.LogSuccess($"{loggerPrefix} Ответ от Яндекса успешно получен");

                logger.LogInformation($"{loggerPrefix} Начало обработки ответа");
                var gameInfoModel = await DeserializeObjectsAsync(
                    httpResponseMessage,
                    logger,
                    cancellationToken);
                logger.LogSuccess($"{loggerPrefix} Ответ успешно обработан");

                result.Add(gameInfoModel);
            }

            return result.ToArray();
        }

        private async Task<GameInfoModel> DeserializeObjectsAsync(
            HttpResponseMessage httpResponseMessage,
            BaseLogger logger,
            CancellationToken cancellationToken)
        {
            var jsonString = await httpResponseMessage.Content
                .ReadAsStringAsync(cancellationToken);

            var jObject = JObject.Parse(jsonString);
            var gameJToken = jObject.GetValue("game") ??
                throw new ArgumentNullException("Не удалось получить объект game из ответа Яндекса");

            var title = gameJToken.GetValue<string>("title");
            var appId = gameJToken.GetValue<int>("appID");
            var firstPublished = gameJToken.GetValue<int>("firstPublished");
            var evaluation = gameJToken.GetValue<double>("rating");
            var playersCount = gameJToken.GetValue<int>("playersCount");
            var categoriesNames = gameJToken.GetValue<string[]>("categoriesNames");
            var rating = gameJToken.GetValue<int?>("gqRating");
            var previewURL = gameJToken.GetValue<string>("media", "cover", "prefix-url");

            var loggerPrefix = $"[{appId}]";

            ProcessNullableField(
                field: title,
                log: () => logger.LogError($"{loggerPrefix} Не удалось получить название"),
                withException: true);

            ProcessNullableField(
                field: previewURL,
                log: () => logger.LogError($"{loggerPrefix} Не удалось получить превью"),
                withException: true);

            ProcessNullableField(
                field: categoriesNames,
                log: () => logger.LogError($"{loggerPrefix} Не удалось получить категории"),
                withException: true);

            ProcessNullableField(
                field: rating,
                log: () => logger.LogError($"{loggerPrefix} Не удалось получить рейтинг"),
                withException: false);

            var gameInfo = new GameInfoModel(
                title: title!,
                appId: appId,
                firstPublished: firstPublished,
                evaluation: evaluation,
                playersCount: playersCount,
                categoriesNames: categoriesNames!,
                previewURL: $"{previewURL!}{PreviewFormat}",
                rating: rating);

            return gameInfo;
        }

        private static void ProcessNullableField<T>(
            T field,
            Action log,
            bool withException)
        {
            if (field is null)
            {
                log();

                if (withException)
                    throw new ArgumentNullException(nameof(field));
            }
        }
    }
}