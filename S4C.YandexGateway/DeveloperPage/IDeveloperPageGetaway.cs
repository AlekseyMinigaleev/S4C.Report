using C4S.DB.Models;
using C4S.Helpers.Logger;
using S4C.YandexGateway.DeveloperPageGateway.Exceptions;
using S4C.YandexGateway.DeveloperPageGateway.Models;

namespace S4C.YandexGateway.DeveloperPageGateway
{
    /// <summary>
    /// Шлюз для получения данных со страницы разработчика
    /// </summary>
    public interface IDeveloperPageGetaway
    {
        // логгер передается как параметр метода, потому что нужен тот же инстанс, что и у вызывающего сервиса.
        
        /// <summary>
        /// Возвращает <see cref="GameInfo"/>[] со страницы разработчика.
        /// </summary>
        /// <param name="gameIds">Массив id <see cref="GameModel"/>, для которых необходимо получить <see cref="GameInfo"/>[]</param>
        /// <param name="logger">Объект <see cref="BaseLogger"/>с помощью, которого будет выполняться логирование</param>
        /// <param name="cancellationToken"><inheritdoc cref="CancellationToken"/></param>
        /// <returns>
        /// <see cref="GameInfo"/>[]
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="InvalidContractException"></exception>
        public Task<GameInfo[]> GetGameInfoAsync(
            int[] gameIds,
            BaseLogger logger,
            CancellationToken cancellationToken = default);
    }
}