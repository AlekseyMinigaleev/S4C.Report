using C4S.DB.Enums;

namespace C4S.DB.Models
{
    public class GameModel
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public GameStatus Status { get; private set; }

        public DateTime PublicationDate { get; private set; }

        public ISet<GamesStatisticModel> GameStatistics { get; private set; }

        private GameModel()
        { }

        public GameModel(int id, string name, DateTime publicationDate)
        {
            Id = id;
            Update(name, publicationDate, GameStatus.New);
        }

        public void Update(string name, DateTime publicationDate, GameStatus status)
        {
            Name = name;
            PublicationDate = publicationDate;
            UpdateStatus(status);
        }

        public void UpdateStatus(GameStatus status) => Status = status;
    }
}