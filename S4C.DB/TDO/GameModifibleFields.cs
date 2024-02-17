using C4S.DB.Models;

namespace C4S.DB.TDO
{
    /// <summary>
    /// Изменяемые поля модели <see cref="GameModel"/>
    /// </summary>
    public class GameModifibleFields
    {
        /// <inheritdoc cref="GameModel.Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="GameModel.PublicationDate"/>
        public DateTime PublicationDate { get; set; }

        /// <inheritdoc cref="GameModel.PreviewURL"/>
        public string PreviewURL { get; set; }

        /// <inheritdoc cref="GameModel.URL"/>
        public IEnumerable<CategoryModel> Categories { get; set; }

        public GameModifibleFields(
            string name,
            DateTime puplicationDate,
            string previewUrl,
            IEnumerable<CategoryModel> category)
        {
            Name = name;
            PublicationDate = puplicationDate;
            PreviewURL = previewUrl;
            Categories = category;
        }

        private GameModifibleFields()
        { }
    }
}