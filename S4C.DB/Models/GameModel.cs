using S4C.DB.Enums;

namespace S4C.DB.Models
{
    public class GameModel
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public GameStatus Status { get; private set; }

        public DateTime PublicationDate { get; private set; }

        public ISet<GamesStatisticModel> GameStatistics { get; private set; }

        private GameModel()
        { }

        public GameModel(string name, DateTime publicationDate)
        {
            Id = Guid.NewGuid();
            Name = name;
            PublicationDate = publicationDate;
            Status = GameStatus.New;
        }

        public void UpdateStatus(GameStatus status) => Status = status;
    }
}