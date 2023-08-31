using S4C.YandexGateway.DeveloperPageGateway.Exceptions;
using S4C.YandexGateway.DeveloperPageGateway.Models;

namespace S4C.YandexGateway.DeveloperPageGateway
{
    /// <summary>
    /// Шлюз для получения данных со страницы разработчика
    /// </summary>
    public interface IDeveloperPageGetaway
    {
        // TODO: что будет если страница разработчика не содрежит игр
        /// <summary>
        /// Возвращает id всех игр со страницы разработчика.
        /// </summary>
        /// <remarks>
        /// Для получения id всех игр, используется статичный парсинг html документа страницы разработчика.
        /// </remarks>
        /// <returns>
        /// <see cref="int"/>[] представляющих id всех игр на странице разработчика.
        /// </returns>
        /// <exception cref="EmptyDeveloperPageException"></exception>
        /// <exception cref="InvalidGameIdException"></exception>
        public Task<int[]> GetGameIdsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает информацию <paramref name="gameIds"/> игр со страницы разработчика.
        /// </summary>
        /// <param name="gameIds"><see cref="int"/>[] представляющих id игр, для которых необходимо получить <see cref="GameInfo"/>[]</param>
        /// <returns>
        /// <see cref="GameInfo"/>[] с детальной информацией об играх
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="InvalidContractException"></exception>
        public Task<GameInfo[]> GetGameInfoAsync(
            int[] gameIds,
            CancellationToken cancellationToken = default);
    }
}