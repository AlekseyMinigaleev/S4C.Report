using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using C4S.Services.Interfaces;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using AngleSharp;
using C4S.DB;
using C4S.Services.Implements.Parsers.ViewModels;
using С4S.API.Extensions;
using C4S.DB.Enums;
using C4S.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace C4S.Services.Implements.Parsers
{
    public class GameParser : IParser
    {
        private readonly IBrowsingContext _browsingContext;
        private readonly ReportDbContext _dbcontext;
        private const string GameDeveloperPageUrl = "https://yandex.ru/games/developer?name=C4S.SHA";
        private const string GameBaseUrl = "https://yandex.ru/games/developer?name=C4S.SHA#app=";


        /*TODO: возможно это как то можно сделать через DI*/
        /*TODO: добавить логер*/

        public GameParser(ReportDbContext dbContext)
        {
            var config = Configuration.Default.WithDefaultLoader();
            _browsingContext = BrowsingContext.New(config);
            _dbcontext = dbContext;
        }

        public async Task ParseAsync()
        {
            var gameList = await GetGameListAsync();
        }

        private async Task<IHtmlCollection<IElement>> GetGameListAsync()
        {
            var document = await _browsingContext.OpenAsync(GameDeveloperPageUrl);

            var gridList = document.QuerySelector(".grid-list");

            var children = gridList.Children;

            return children;
        }

        private async Task<List<string>> GetGameUrlsAsync(IHtmlCollection<IElement> elements)
        {
            var gameUrls = new List<string>();

            foreach (var item in elements)
            {
                var gameUrlElement = item.QuerySelector(".game-url") as IHtmlAnchorElement;
                var path = gameUrlElement.PathName;
                int lastIndex = path.LastIndexOf("\\");
                string gameId = path.Substring(lastIndex + 1);
                await GetGame(gameId);
            }

            return gameUrls;
        }

        private async Task GetGame(string gameId)
        {
            var gameUrl = $"{GameBaseUrl}{gameId}";

            using IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(gameUrl);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            var gameTitleElement = wait.Until(drv => drv.FindElement(By.CssSelector(".game-page__title")));
            var gameFeaturesItems = wait.Until(drv => drv.FindElements(By.CssSelector(".game-features__item")));

            var publicationDateElement = gameFeaturesItems
                .Where(x => x.FindElement(By.CssSelector(".game-features__name")).Text == "Дата выхода")
                .Single();

            var gameInfoAsString = new GameInfoAsStrng
            {
                Id = gameId,
                Name = gameTitleElement.Text,
                Status = "1",
                PublicationDate = publicationDateElement.Text,
            };

            await UpdateDB(gameInfoAsString);
        }

        private async Task UpdateDB(GameInfoAsStrng gameInfoAsStrng)
        {
            var gameId = int.Parse(gameInfoAsStrng.Id);
            DateTimeExtenstions.TryParseYandexDate(gameInfoAsStrng.PublicationDate, out var result);
            var publicationDate = result;
            var gamestatus = int.Parse(gameInfoAsStrng.Status);

            var a = await _dbcontext.GameModels.SingleOrDefaultAsync(x => x.Id == gameId);

            if(a is null)
            {
                var gameModel = new GameModel(gameId, gameInfoAsStrng.Name, (DateTime)result);

                _dbcontext.GameModels.Add(gameModel);
            }
            else
            {
                a.Update(gameInfoAsStrng.Name, (DateTime)result, (GameStatus) gamestatus);
            }

            await _dbcontext.SaveChangesAsync();
        }
    }
}
