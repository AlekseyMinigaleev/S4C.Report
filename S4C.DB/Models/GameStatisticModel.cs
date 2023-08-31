namespace C4S.DB.Models
{
    /// <summary>
    /// Сущность игровой статистики
    /// </summary>
    public class GameStatisticModel
    {
        /// <summary>
        /// Id сущности
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Связь c таблицей <see cref="GameModel"/>
        /// </summary>
        public int GameId { get; private set; }
        public GameModel Game { get; private set; }

        /*TODO: сделать отдельную таблицу*/
        #region game statuses
        /// <summary>
        /// Статус новой игры
        /// </summary>
        public bool IsNew { get; private set; }
        /// <summary>
        /// Статус продвигаемой игры
        /// </summary>
        public bool IsPromoted { get; private set; }
        #endregion 

        /// <summary>
        /// Оценка игры
        /// </summary>
        public double? Evaluation { get; private set; }
        
        /// <summary>
        /// Количество игроков
        /// </summary>
        public int PlayersCount { get; private set; }
        
        /// <summary>
        /// Дата последней синхронизации с яндексом
        /// </summary>
        public DateTime LastSynchroDate { get; private set; }

        private GameStatisticModel()
        { }

        public GameStatisticModel(GameModel game,
            int playersCount,
            bool isNew,
            bool isPromoted,
            DateTime lastSynchroDate,
            double? evaluation = default)
        {
            GameId = game.Id;
            Game = game;
            IsNew = isNew;
            IsPromoted = isPromoted;
            PlayersCount = playersCount;
            LastSynchroDate = lastSynchroDate;
            Evaluation = evaluation;
        }
    }
}
