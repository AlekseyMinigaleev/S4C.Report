namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица связей <see cref="GameModel"/> - <see cref="CategoryModel"/>
    /// </summary>
    public class CategoryGameModel
    {
        /// <summary>
        /// Связь с таблицей <see cref="GameModel"/>
        /// </summary>
        public GameModel Game { get; private set; }

        /// <summary>
        /// PK и FK <see cref="GameModel"/>
        /// </summary>
        public Guid GameId { get; private set; }

        /// <summary>
        /// Связь с таблицей <see cref="CategoryModel"/>
        /// </summary>
        public CategoryModel Category { get; private set; }

        /// <summary>
        /// PK и FK <see cref="CategoryModel"/>
        /// </summary>
        public Guid CategoryId { get; private set; }

        public CategoryGameModel(
            GameModel gameStatistic,
            CategoryModel status)
        {
            Game = gameStatistic;
            GameId = gameStatistic.Id;
            Category = status;
            CategoryId = status.Id;
        }

        private CategoryGameModel()
        { }
    }
}