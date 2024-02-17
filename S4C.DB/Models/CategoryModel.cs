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
    
        /*TODO: переопределить Equals в этой модели, изменить название метода, сделать его дженериком и вынести в Helpers, добавить документацию*/
        public static IEnumerable<CategoryModel> GetItemsNotInCollection(
          IEnumerable<CategoryModel> firstCollection,
          IEnumerable<CategoryModel> secondCollection)
        {
            var itemsNotInCollection = firstCollection
                .Where(firstItem => !secondCollection.Any(secondItem => secondItem.Name == firstItem.Name));
            return itemsNotInCollection;
        }

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