using Newtonsoft.Json;
using System.Text;

namespace S4C.YandexGateway.DeveloperPageGateway
{
    public static class HttpRequestMethods
    {
        /*TODO: hardcode*/
        private static readonly string requestUri = "https://yandex.ru/games/_crpd/2fxg324u1/0463a9o-i/WmeCEDy-VUd2S-vlZ9bXdhXct8xMhg5RScdAENGPZD0MTYp-CnAWBj63mxRiHCIVXFZeDaAw-_pk77fa1o_b6g4Xg239frXHxqdDTPsCJvJt3LvIFdsyWTgHsRSlg2gFVAn_fn8Fai4yy38QRgReUCh-OlGBcNfvWLe74sbqz5Zec-MdKb4RC2o7w6B3xtYebdzT9AWKalVo_1w4uaM0ZTRFyxZfGXY2fqdDaGoYZhAYXmJlpUjnxzjnm7qCgo-iAmfXZTHqURtCO-uoV9q6KgHwv7hZsgJROONgJLXfBAl0VbtaeyV2LmKrXxBiECIYEHJSPa0Ip_N4z8-CmsLHkg5vx31Bsh0LWj-buBuKzm45jOv8PZoyHTjHDAytsxw9IC2zIgc1ek4ugztIQgBeUEACDlHlYIuLZN_7_sb6s_5uG-M1WcYtV2Ybv7B31qZSAZiv_BH6MglQu3AklesQUVA1-3IDMQJM";

        public static HttpRequestMessage GetGameInfo(int[] appIDs, string format)
        {
            var requestData = new
            {
                appIDs,
                format
            };

            var jsonPayload = JsonConvert.SerializeObject(requestData);

            var payload = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var result = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = payload
            };

            return result;
        }
    }
}
