namespace C4S.DB.Models
{
    /// <summary>
    /// Сущность игры
    /// </summary>
    public class GameModel
    {
        /// <summary>
        /// Id игры
        /// </summary>
        /// <remarks>
        /// Должно быть такое же как и на странице разработчика
        /// </remarks>
        public int Id { get; private set; }

        /// <summary>
        /// Название игры
        /// </summary>
        public string? Name { get; private set; }

        /// <summary>
        /// Дата публикации
        /// </summary>
        public DateTime? PublicationDate { get; private set; }

        public ISet<GameStatisticModel> GameStatistics { get; private set; }

        private GameModel()
        { }

        public GameModel(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Выполняет обновление сущности
        /// </summary>
        /// <param name="name">Название игры</param>
        /// <param name="publicationDate">дата публикации</param>
        public void Update(string name, DateTime publicationDate)
        {
            Name = name;
            PublicationDate = publicationDate;
        }
    }
}