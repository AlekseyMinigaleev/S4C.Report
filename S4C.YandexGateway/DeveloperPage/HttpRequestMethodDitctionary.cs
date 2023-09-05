using C4S.DB.Models;
using Newtonsoft.Json;
using S4C.YandexGateway.DeveloperPage.Models;
using System.Text;

namespace S4C.YandexGateway.DeveloperPage
{
    /// <summary>
    /// Словарь, содержащий все <see cref="HttpRequestMessage"/>, необходимые для получения данных
    /// </summary>
    public static class HttpRequestMethodDitctionary
    {
        /// <summary>
        /// Создает <see cref="HttpRequestMessage"/> для получения <see cref="GameInfoModel"/>.
        /// </summary>
        /// <param name="requestUrl">url запроса</param>
        /// <param name="appIDs">массив, содержащий id <see cref="GameModel"/>, по которым необходимо получить информацию</param>
        /// <param name="format">формат ответа</param>.
        /// <returns><see cref="HttpRequestMessage"/></returns>.
        public static HttpRequestMessage GetGamesInfo(string requestUrl, int[] appIDs, string format)
        {
            var requestData = new
            {
                appIDs = appIDs,
                format = format,
            };

            var jsonPayload = JsonConvert.SerializeObject(requestData);

            var payload = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var result = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = payload
            };

            return result;
        }
    }

    /*TODO:заменить на enum*/
    /// <summary>
    /// Словарь всех допустимых форматов для запроса на Яндекс игры
    /// </summary>
    public static class HttpRequestDictionary
    {
        /// <summary>
        /// Полный формат
        /// </summary>
        public static string LongFormat = "long";

        /// <summary>
        /// Короткий формат
        /// </summary>
        public static string ShortFormat = "short";
    }
}