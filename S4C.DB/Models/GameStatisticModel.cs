namespace C4S.DB.Models
{
    public class GameStatisticModel
    {
        public Guid Id { get; private set; }
        
        public int GameId { get; private set; }
        public GameModel Game { get; private set; }

        /*TODO: сделать отдельную таблицу*/
        #region game statuses
        public bool IsNew { get; private set; }
        public bool IsPromoted { get; private set; }
        #endregion 

        public double? Evaluation { get; private set; }
        
        public int PlayersCount { get; private set; }
        
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
