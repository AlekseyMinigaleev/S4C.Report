namespace C4S.DB.Models
{
    public class GamesStatisticModel
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }
        public GameModel Game { get; set; }

        public double? Evaluation { get; set; }

        public int PlayersCount { get; set; }

        public DateTime LastSynchroDate { get; set; }

        private GamesStatisticModel()
        { }

        public GamesStatisticModel(GameModel game,
            int playersCount,
            DateTime lastSynchroDate,
            double? evaluation = default)
        {
            GameId = game.Id;
            Game = game;
            PlayersCount = playersCount;
            LastSynchroDate = lastSynchroDate;
            Evaluation = evaluation;
        }

        public void UpdateLastSynhroDate(DateTime lastSynchroDate) => LastSynchroDate = lastSynchroDate;
    }
}
