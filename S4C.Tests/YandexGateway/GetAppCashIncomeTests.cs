using C4S.Common.Models;
using Moq;
using Moq.Protected;
using S4C.YandexGateway.RSYA;
using System.Net;
using Xunit;

namespace S4C.Tests.YandexGateway
{
    public class GetAppCashIncomeTests
    {
        [Theory]
        [InlineData(1656583, "{ \"data\" : { \"currencies\" : [ { \"code\" : \"USD\", \"id\" : \"1\" }, { \"code\" : \"RUB\", \"id\" : \"2\" }, { \"code\" : \"EUR\", \"id\" : \"3\" }, { \"code\" : \"CHF\", \"id\" : \"756\" } ], \"dimensions\" : {}, \"is_last_page\" : true, \"measures\" : { \"partner_wo_nds\" : { \"currency\" : \"RUB\", \"index\" : 1, \"title\" : \"Вознаграждение\", \"type\" : \"money\", \"unit\" : \"money\" } }, \"periods\" : [ [ \"2023-09-19\", \"2023-09-20\" ] ], \"points\" : [ { \"dimensions\" : {}, \"measures\" : [ { \"partner_wo_nds\" : 2.64 } ] } ], \"report_title\" : \"Отчет за период 19.09.2023 - 20.09.2023\", \"total_rows\" : 1, \"totals\" : { \"2\" : [ { \"partner_wo_nds\" : 2.64 } ] } }, \"result\" : \"ok\" }")]
        [InlineData(123, "{ \"data\" : { \"currencies\" : [ { \"code\" : \"USD\", \"id\" : \"1\" }, { \"code\" : \"RUB\", \"id\" : \"2\" }, { \"code\" : \"EUR\", \"id\" : \"3\" }, { \"code\" : \"CHF\", \"id\" : \"756\" } ], \"dimensions\" : {}, \"is_last_page\" : true, \"measures\" : {}, \"periods\" : [ [ \"2023-09-19\", \"2023-09-20\" ] ], \"points\" : [], \"report_title\" : \"Отчет за период 19.09.2023 - 20.09.2023\", \"total_rows\" : 0, \"totals\" : { \"2\" : [ null ] } }, \"result\" : \"ok\" }")]
        public async Task GetsCashIncomeTest(int pageId, string content)
        {
            var rsyaGateway = new RsyaGateway(GetHttpClientFactory(content));
            var period = new DateTimeRange(new DateTime(2023, 9, 19), new DateTime(2023, 9, 20));

            var result = await rsyaGateway.GetAppCashIncomeAsync(
                pageId,
                "y0_AgAAAABf6WyfAAbJ0QAAAADsuO9IoICcpinRSf6pviKLtFIrKvQDM4c",
                period);

            if(pageId == 1656583)
                Assert.Equal(2.64, result);

            if (pageId == 123)
                Assert.Null(result);
        }

        private IHttpClientFactory GetHttpClientFactory(string content)
        {
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content)
                });
            var httpClient = new HttpClient(mockMessageHandler.Object);
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            return httpClientFactoryMock.Object;
        }
    }
}
