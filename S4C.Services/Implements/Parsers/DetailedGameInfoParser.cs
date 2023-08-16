using C4S.DB.Models;
using C4S.Services.Helpers;
using C4S.Services.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using С4S.API.Extensions;

namespace C4S.Services.Implements.Parsers
{
    public class DetailedGameInfoParser : IDetailedGameInfoParser
    {
        public const string BaseUrl = "https://yandex.ru/games/developer?name=C4S.SHA#app=";

        public string DetailedGameUrl { get; private set; }

        public int GameId { get; private set; }


        public void SetUrl(string gameId)
        {
            DetailedGameUrl = $"{BaseUrl}{gameId}";

            var gameIdParsingResult = int.TryParse(gameId, out var result);

            ParsersHelpers.ThrowIf(!gameIdParsingResult, "Не удалось конвертировать Id");

            GameId = result;
        }

        public GameModel GetDetailedGameInfo()
        {
            var (gameTitleElement, gameFeaturesElements) = ParseDetailedGameInfo(DetailedGameUrl);

            ParsersHelpers.ThrowIfNull(gameTitleElement, "Не удалось получить название игры");
            ParsersHelpers.ThrowIfNull(gameFeaturesElements, "Не удалось получить featuresElements игры");

            var publicationDate = GetPublicationDate(gameFeaturesElements);
            var gameModel = new GameModel(GameId, gameTitleElement.Text, publicationDate);

            return gameModel;
        }

        public Task ParseGameStatistic()
        {
            throw new NotImplementedException();
        }

        private (IWebElement?, IReadOnlyCollection<IWebElement?>) ParseDetailedGameInfo(string gameUrl)
        {
            using IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(gameUrl);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            var gameTitleElement = wait.Until(drv => drv.FindElement(By.CssSelector(".game-page__title")));
            var gameFeaturesElements = wait.Until(drv => drv.FindElements(By.CssSelector(".game-features__item")));

            return (gameTitleElement, gameFeaturesElements);
        }

        private DateTime GetPublicationDate(IReadOnlyCollection<IWebElement> gameFeaturesElements)
        {
            //составление ответа
            var publicationDateElement = gameFeaturesElements
                .Where(x => x.FindElement(By.CssSelector(".game-features__name")).Text == "Дата выхода")
                .Single();

            var publicationDateParseResult = DateTimeExtenstions
                 .TryParseYandexDate(publicationDateElement.Text, out var dateTime);

            ParsersHelpers.ThrowIf(!publicationDateParseResult, "Не удалось конвертировать дату публикации");

            return (DateTime)dateTime;
        }
    }
}