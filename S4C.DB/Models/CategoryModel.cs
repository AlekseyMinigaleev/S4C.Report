namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица статуса игры
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Имя категории
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Название статуса
        /// </summary>
        public string Title { get; private set; }

        public CategoryModel(
            string name,
            string title)
        {
            Id = Guid.NewGuid();
            Name = name;
            Title = title;
        }

        private CategoryModel()
        { }
    }
}