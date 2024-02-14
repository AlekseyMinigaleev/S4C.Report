using C4S.DB.Models;
using C4S.Helpers.Extensions;
using Newtonsoft.Json;
using S4C.YandexGateway.DeveloperPage.Enums;
using S4C.YandexGateway.DeveloperPage.Models;
using System.Text;

namespace S4C.YandexGateway.DeveloperPage
{
    /// <summary>
    /// Словарь, содержащий все <see cref="HttpRequestMessage"/>, необходимые для получения данных по игре со страницы разработчика
    /// </summary>
    public static class HttpRequestMethodDitctionary
    {
        /// <summary>
        /// Создает <see cref="HttpRequestMessage"/> для получения <see cref="GameInfoModel"/>.
        /// </summary>
        /// <param name="requestUrl">url запроса</param>
        /// <param name="appID">id <see cref="GameModel"/>, по которому необходимо получить информацию</param>
        /// <param name="format">формат ответа</param>.
        /// <returns><see cref="HttpRequestMessage"/></returns>.
        public static HttpRequestMessage GetGamesInfo(string requestUrl, int appID, RequestFormat format)
        {
            var requestData = new
            {
                appID = appID,
                format = format.GetName(),
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
}