namespace S4C.YandexGateway.DeveloperPageGateway.Models
{
    public class GameInfo
    {
        public string Name { get; set; }

        public int AppId { get; set; }

        public int FirstPublished { get; set; }

        public double Rating { get; set; }

        public int PlayersCount { get; set; }

        public string[] CategoriesNames { get; set; }

        public GameInfo(string name,
            int appId,
            int firstPublished,
            double rating,
            int playersCount,
            string[] categoriesNames)
        {
            AppId = appId;
            FirstPublished = firstPublished;
            Rating = rating;
            PlayersCount = playersCount;
            CategoriesNames = categoriesNames;
        }
    }
}
