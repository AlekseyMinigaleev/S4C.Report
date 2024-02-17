using C4S.Common.Models;
using C4S.Helpers.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using S4C.Helpers;

namespace S4C.YandexGateway.RSYA
{
    /// <inheritdoc cref="IRsyaGateway"/>
    public class RsyaGateway : IRsyaGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RsyaGateway(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc/>
        public async Task<double?> GetAppCashIncomeAsync(
            int pageId,
            string authorization,
            DateTimeRange period,
            CancellationToken cancellationToken = default)
        {
            HttpRequestMessage createRequest() =>
                HttpRequestsMethodDictionary.GetAppReport(
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
                .SingleOrDefault();

            return cashIncome?.Value;
        }

        private class CashIncome
        {
            [JsonProperty("partner_wo_nds")]
            public double Value { get; set; }
        }
    }
}