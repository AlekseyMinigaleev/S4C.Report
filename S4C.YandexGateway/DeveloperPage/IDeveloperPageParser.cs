using C4S.Helpers.Logger;
using S4C.YandexGateway.DeveloperPage.Exceptions;

namespace S4C.YandexGateway.DeveloperPage
{
    /// <summary>
    /// Класс выполняющий парсинг html документа, страницы разработчика.
    /// </summary>
    public interface IDeveloperPageParser
    {
        // TODO: что будет если страница разработчика не содрежит игр
        /// <summary>
        /// Возвращает id всех игр со страницы разработчика.
        /// </summary>
        /// <remarks>
        /// Для получения id всех игр, используется статичный парсинг html документа, страницы разработчика.
        /// </remarks>
        /// <param name="logger">Объект <see cref="BaseLogger"/>с помощью, которого будет выполняться логирование</param>
        /// <param name="developerPageUrl">Ссылка на страницу разработчика</param>
        /// <param name="cancellationToken"><inheritdoc cref="CancellationToken"/></param>
        /// <returns>
        /// <see cref="int"/>[] представляющий id всех игр на странице разработчика.
        /// </returns>
        /// <exception cref="EmptyDeveloperPageException"></exception>
        /// <exception cref="InvalidGameIdException"></exception>
        public Task<int[]> GetGameIdsAsync(
            string developerPageUrl,
            BaseLogger logger,
            CancellationToken cancellationToken = default);
    }
}