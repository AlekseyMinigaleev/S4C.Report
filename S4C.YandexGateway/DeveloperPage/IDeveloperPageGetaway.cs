using C4S.DB.Models;
using C4S.Helpers.Logger;
using S4C.YandexGateway.DeveloperPage.Exceptions;
using S4C.YandexGateway.DeveloperPage.Models;

namespace S4C.YandexGateway.DeveloperPage
{
    /// <summary>
    /// Шлюз для получения данных со страницы разработчика
    /// </summary>
    public interface IDeveloperPageGetaway
    {
        // логгер передается как параметр метода, потому что нужен тот же инстанс, что и у вызывающего сервиса.
        
        /// <summary>
        /// Возвращает <see cref="GameInfoModel"/>[] со страницы разработчика.
        /// </summary>
        /// <param name="gameIds">Массив id <see cref="GameModel"/>, для которых необходимо получить <see cref="GameInfoModel"/>[]</param>
        /// <param name="logger">Объект <see cref="BaseLogger"/>с помощью, которого будет выполняться логирование</param>
        /// <param name="cancellationToken"><inheritdoc cref="CancellationToken"/></param>
        /// <returns>
        /// <see cref="GameInfoModel"/>[]
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="InvalidContractException"></exception>
        public Task<GameInfoModel[]> GetGameInfoAsync(
            int[] gameIds,
            BaseLogger logger,
            CancellationToken cancellationToken = default);
    }
}