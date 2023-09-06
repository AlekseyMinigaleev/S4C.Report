namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица связей <see cref="GameStatisticModel"/> - <see cref="GameStatusModel"/>
    /// </summary>
    public class GameGameStatusModel
    {
        /// <summary>
        /// Связь с таблицей <see cref="GameStatisticModel"/>
        /// </summary>
        public GameStatisticModel GameStatistic { get; private set; }

        /// <summary>
        /// PK и FK <see cref="GameStatisticModel"/>
        /// </summary>
        public Guid GameStatisticId { get; private set; }

        /// <summary>
        /// Связь с таблицей <see cref="GameStatusModel"/>
        /// </summary>
        public GameStatusModel GameStatus { get; private set; }

        /// <summary>
        /// PK и FK <see cref="GameStatusModel"/>
        /// </summary>
        public Guid GameStatusId { get; set; }


        public GameGameStatusModel(
            GameStatisticModel gameStatistic,
            GameStatusModel status)
        {
            GameStatistic = gameStatistic;
            GameStatisticId = gameStatistic.Id;
            GameStatus = status;
            GameStatusId = status.Id;
        }

        private GameGameStatusModel()
        { }
    }
}
