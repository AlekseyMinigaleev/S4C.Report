namespace С4S.API.Models
{
    /// <summary>
    /// Параметры для пагинации
    /// </summary>
    public class Paginate
    {
        /// <summary>
        /// Количество элементов на странице
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// номер страницы
        /// </summary>
        public int PageNumber { get; set; }
    }
}