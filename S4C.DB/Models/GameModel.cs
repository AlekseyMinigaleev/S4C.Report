namespace C4S.DB.Models
{
    public class GameModel
    {
        public int Id { get; private set; }

        public string? Name { get; private set; }

        public DateTime? PublicationDate { get; private set; }

        public ISet<GameStatisticModel> GameStatistics { get; private set; }

        private GameModel()
        { }

        public GameModel(int id)
        {
            Id = id;
        }

        public void Update(string name, DateTime publicationDate)
        {
            Name = name;
            PublicationDate = publicationDate;
        }
    }
}