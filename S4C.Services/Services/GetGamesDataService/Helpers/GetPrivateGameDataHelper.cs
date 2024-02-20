using C4S.Common.Models;
using C4S.DB.Models;
using C4S.Helpers.Extensions;
using C4S.Services.Services.GetGamesDataService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using S4C.Helpers;

namespace C4S.Services.Services.GetGamesDataService.Helpers
{
    /// <summary>
    /// Вспомогательный класс для получения <see cref="PrivateGameData"/> игры.
    /// </summary>
    public class GetPrivateGameDataHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GetPrivateGameDataHelper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Получает <see cref="PrivateGameData"/> игры.
        /// </summary>
        /// <param name="pageId">Идентификатор страницы игры.</param>
        /// <param name="authorization">Токен авторизации для доступа к конфиденциальным данным.</param>
        /// <param name="period">Период, за который запрашиваются данные.</param>
        /// <param name="cancellationToken">Токен отмены задачи.</param>
        /// <returns>Объект <see cref="PrivateGameData"/>, содержащий конфиденциальные данные игры.</returns>
        public async Task<PrivateGameData> GetPrivateGameDataAsync(
            int pageId,
            string authorization,
            DateTimeRange period,
            CancellationToken cancellationToken = default)
        {
            HttpRequestMessage createRequest() =>
                PartnerInterfaceHttpRequestsMethodDictionary.GetAppReport(
                    authorization,
                    pageId,
                    period);

            var httpResponseMessage = await HttpUtils
                .SendRequestAsync(
                    createRequest,
                    _httpClientFactory,
                    cancellationToken);

            var jsonString = await httpResponseMessage.Content
                .ReadAsStringAsync(cancellationToken);

            var jObject = JObject.Parse(jsonString);

            var cashIncome = jObject
                .GetValue<JToken>("data")
                .GetValue<JToken>("totals")
                .GetValue<JArray>("2")
                .ToObject<CashIncome[]>()!
                .Single();

            var result = new PrivateGameData { CashIncome = cashIncome?.Value };

            return result;
        }

        private class CashIncome
        {
            [JsonProperty("partner_wo_nds")]
            public double Value { get; set; }
        }
    }
}