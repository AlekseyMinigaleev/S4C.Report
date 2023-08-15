using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using C4S.Services.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace C4S.Services.Implements.Parsers
{
    public class GameStatisticParser : IParser
    {
        private readonly IBrowsingContext _browsingContext;
        private const string GameDeveloperPageUrl = "https://yandex.ru/games/developer?name=C4S.SHA";

        /*TODO: возможно это как то можно сделать через DI*/
        /*TODO: добавить логер*/

        public GameStatisticParser()
        {
            var config = Configuration.Default.WithDefaultLoader();
            _browsingContext = BrowsingContext.New(config);
        }

        public async Task ParseAsync()
        {

        }

        private async Task<IHtmlCollection<IElement>> GetGameListAsync()
        {
            var document = await _browsingContext.OpenAsync(GameDeveloperPageUrl);

            var gridList = document.QuerySelector(".grid-list");

            var children = gridList.Children;

            return children;
        }

        private List<string> GetGameUrls(IHtmlCollection<IElement> elements)
        {
            var gameUrls = new List<string>();

            foreach (var item in elements)
            {
                var gameUrlElement = item.QuerySelector(".game-url") as IHtmlAnchorElement;
                var path = gameUrlElement.PathName;
                int lastIndex = path.LastIndexOf("\\");
                string gameId = path.Substring(lastIndex + 1);

                gameUrls.Add($"https://yandex.ru/games/developer?name=C4S.SHA#app={gameId}");
            }

            return gameUrls;
        }

        private async Task GetGameStatistic(string gameUrl)
        {
            using IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(gameUrl);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            var playersCountElement = wait.Until(drv => drv.FindElement(By.CssSelector(".game-number__number-text")));
            var evaluationElement = wait.Until(drv => drv.FindElement(By.CssSelector(".RatingBlock-RatingValue")));

            var playersCountText = playersCountElement.Text;
            var evaluationText = evaluationElement.Text;
        }
    }
}