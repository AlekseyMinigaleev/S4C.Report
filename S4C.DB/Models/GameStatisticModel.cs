using System.Linq.Expressions;

namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица игровой статистики
    /// </summary>
    public class GameStatisticModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Связь с <see cref="GameModel"/>
        /// </summary>
        public GameModel Game { get; private set; }

        /// <summary>
        /// FK <see cref="GameModel"/>
        /// </summary>
        public Guid GameId { get; set; }

        /// <summary>
        /// Оценка игры
        /// </summary>
        public double Evaluation { get; private set; }

        /// <summary>
        /// Рейтинг игры
        /// </summary>
        public int? Rating { get; set; }

        /// <summary>
        /// Количество игроков
        /// </summary>
        public int PlayersCount { get; private set; }

        /// <summary>
        /// Доход
        /// </summary>
        public double? CashIncome { get; private set; }

        /// <summary>
        /// Дата последней синхронизации с Яндексом
        /// </summary>
        public DateTime LastSynchroDate { get; private set; }

        /// <summary>
        /// Список статусов игры
        /// </summary>
        public ISet<GameStatusModel> Statuses =>
            GameGameStatus.Select(x => x.GameStatus).ToHashSet();

        /// <summary>
        /// Список связей <see cref="GameModel"/> - <see cref="GameStatusModel"/>
        /// </summary>
        public ISet<GameGameStatusModel> GameGameStatus
        {
            get
            {
                return _gameGameStatus ?? new HashSet<GameGameStatusModel>();
            }
            set { _gameGameStatus = value; }
        }

        private ISet<GameGameStatusModel>? _gameGameStatus;

        public GameStatisticModel(
            GameModel game,
            int playersCount,
            DateTime lastSynchroDate,
            double evaluation,
            ISet<GameStatusModel> statuses)
        {
            Id = Guid.NewGuid();
            GameId = game.Id;
            Game = game;
            AddStatuses(statuses);
            PlayersCount = playersCount;
            LastSynchroDate = lastSynchroDate;
            Evaluation = evaluation;
        }

        private GameStatisticModel()
        { }

        /// <summary>
        /// Изменяет список статусов игры на <paramref name="statuses"/>
        /// </summary>
        /// <param name="statuses">Новый список статусов игры</param>
        public void AddStatuses(ISet<GameStatusModel> statuses)
        {
            GameGameStatus.Clear();
            foreach (var status in statuses)
                AddStatus(status);
        }

        /// <summary>
        /// Добавляет <paramref name="status"/> к списку статусов игры
        /// </summary>
        /// <param name="status">Добавляемый статус</param>
        public void AddStatus(GameStatusModel status) =>
            GameGameStatus.Add(new GameGameStatusModel(this, status));
    }

    /// <summary>
    /// Справочник <see cref="Expression"/> для <see cref="GameStatisticModel"/>
    /// </summary>
    public static class GameStatisticExpression
    {
        public static readonly Expression<Func<GameStatisticModel, string>> GetStatusesAsStringExpression = (gameStatistic) =>
            gameStatistic.Statuses.Count() == 0
                ? "-"
                : string.Join(", ", gameStatistic);
    }
}