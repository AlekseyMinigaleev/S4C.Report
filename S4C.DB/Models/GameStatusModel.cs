namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица статуса игры
    /// </summary>
    public class GameStatusModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название статуса
        /// </summary>
        public string Name { get; set; }

        public GameStatusModel(
            string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        private GameStatusModel()
        { }
    }
}