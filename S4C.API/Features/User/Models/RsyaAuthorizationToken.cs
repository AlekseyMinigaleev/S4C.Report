using FluentValidation;
using System.Text.RegularExpressions;

namespace С4S.API.Features.User.Requests
{
    public class RsyaAuthorizationToken
    {
        public RsyaAuthorizationToken(string token)
        {
            Token = token;
        }

        /// <summary>
        /// Токен авторизации для апи /partner2.yandex.ru/api
        /// </summary>
        public string Token { get; set; }
    }

    public class RsyaAuthorizationTokenValidator : AbstractValidator<RsyaAuthorizationToken>
    {
        public RsyaAuthorizationTokenValidator(IHttpClientFactory httpClientFactory)
        {
            RuleFor(x => x.Token)
                .MustAsync(async (authorizationToken, cancellationToken) =>
                    {
                        var validPattern = new Regex("^[a-zA-Z0-9_]+$");
                        if (!validPattern.IsMatch(authorizationToken))
                            return false;

                        var testUrl = "https://partner2.yandex.ru/api/statistics2/tree.json?lang=ru";

                        var httpRequestMethod = new HttpRequestMessage(HttpMethod.Get, testUrl);
                        httpRequestMethod.Headers.Add("Authorization", authorizationToken);

                        using var httpClient = httpClientFactory.CreateClient();
                        var response = await httpClient.SendAsync(httpRequestMethod, cancellationToken);

                        return response.IsSuccessStatusCode;
                    })
                .WithMessage("Указан неверный токен авторизации для апи /partner2.yandex.ru/api")
                .WithErrorCode($"rsyaAuthorizationToken");
        }
    }   
}