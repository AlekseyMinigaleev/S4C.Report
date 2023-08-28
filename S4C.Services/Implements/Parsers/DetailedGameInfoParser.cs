using C4S.DB.Models;
using C4S.Services.Helpers;
using C4S.Services.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using С4S.API.Extensions;

namespace C4S.Services.Implements.Parsers
{
    //public class DetailedGameInfoParser : IDetailedGameInfoParser
    //{
    //    public const string BaseUrl = "https://yandex.ru/games/developer?name=C4S.SHA#app=";

    //    public string DetailedGameUrl { get; private set; }

    //    public int GameId { get; private set; }


    //    public void SetUrl(string gameId)
    //    {
    //        DetailedGameUrl = $"{BaseUrl}{gameId}";

    //        var gameIdParsingResult = int.TryParse(gameId, out var result);

    //        ParsersHelpers.ThrowIf(!gameIdParsingResult, "Не удалось конвертировать Id");

    //        GameId = result;
    //    }

    //    public GameModel GetDetailedGameInfo()
    //    {
    //       // тут почему то объкты диспосятся, если делать все в 1 методе то ве работает мб надо синхронизовать потоки
    //        var (gameTitleElement, gameFeaturesElements) = ParseDetailedGameInfo(DetailedGameUrl);

    //        ParsersHelpers.ThrowIfNull(gameTitleElement, "Не удалось получить название игры");
    //        ParsersHelpers.ThrowIfNull(gameFeaturesElements, "Не удалось получить featuresElements игры");

    //        var publicationDate = GetPublicationDate(gameFeaturesElements);
    //        var gameModel = new GameModel(GameId, gameTitleElement.Text, publicationDate);

    //        return gameModel;
    //    }

    //    public Task ParseGameStatistic()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private static void WaitForPageLoad(IWebDriver driver)
    //    {
    //        var wait = new WebDriverWait( driver, TimeSpan.FromMinutes(10));
    //        wait.Until(webDriver => ((IJavaScriptExecutor)webDriver).ExecuteScript("return document.readyState").Equals("complete"));
    //    }

    //    private (IWebElement?, IReadOnlyCollection<IWebElement?>) ParseDetailedGameInfo(string gameUrl)
    //    {
    //        using IWebDriver driver = new ChromeDriver();
    //        driver.Navigate().GoToUrl(gameUrl);
    //        // проверить
    //        WaitForPageLoad(driver);
    //        //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
    //        var gameTitleElement = driver.FindElement(By.CssSelector(".game-page__title"));
    //        var gameFeaturesElements = driver.FindElements(By.CssSelector(".game-features__item"));

    //        return (gameTitleElement, gameFeaturesElements);
    //    }

    //    private DateTime GetPublicationDate(IReadOnlyCollection<IWebElement> gameFeaturesElements)
    //    {
    //        //составление ответа
    //        var publicationDateElement = gameFeaturesElements
    //            .Where(x => x.FindElement(By.CssSelector(".game-features__name")).Text == "Дата выхода")
    //            .Single();

    //        var publicationDateParseResult = DateTimeExtenstions
    //             .TryParseYandexDate(publicationDateElement.Text, out var dateTime);

    //        ParsersHelpers.ThrowIf(!publicationDateParseResult, "Не удалось конвертировать дату публикации");

    //        return (DateTime)dateTime;
    //    }
    //}
}