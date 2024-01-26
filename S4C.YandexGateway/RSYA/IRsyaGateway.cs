using C4S.Common.Models;

namespace S4C.YandexGateway.RSYA
{
    /// <summary>
    /// Шлюз для получения данных с РСЯ
    /// </summary>
    public interface IRsyaGateway
    {
        /// <summary>
        /// Возвращает доход, который принесла игра.
        /// </summary>
        /// <param name="pageId">Идентификатор страницы игры.</param>
        /// <param name="authorization">Токен авторизации.</param>
        /// <param name="period">Период, за который запрашивается статистика.</param>
        /// <returns>Доход, который принесла игра.</returns>
        public Task<double?> GetAppCashIncomeAsync(
            int pageId,
            string authorization,
            DateTimeRange period,
            CancellationToken cancellationToken = default);
    }
}