using C4S.DB.Enums;

namespace C4S.DB.Models
{
    public class GamesStatisticModel
    {
        public Guid Id { get; private set; }
        
        public int GameId { get; private set; }
        public GameModel Game { get; private set; }
        
        public GameStatus Status { get; private set; }
        
        public double? Evaluation { get; private set; }
        
        public int PlayersCount { get; private set; }
        
        public DateTime LastSynchroDate { get; private set; }

        private GamesStatisticModel()
        { }

        public GamesStatisticModel(GameModel game,
            int playersCount,
            GameStatus status,
            DateTime lastSynchroDate,
            double? evaluation = default)
        {
            GameId = game.Id;
            Game = game;
            Status = status;
            PlayersCount = playersCount;
            LastSynchroDate = lastSynchroDate;
            Evaluation = evaluation;
        }
    }
}
