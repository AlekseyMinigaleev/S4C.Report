namespace S4C.YandexGateway.DeveloperPage.Exceptions
{
    /// <summary>
    /// Ошибка указывающая, на то что страница разработчика не содержит игр
    /// </summary>
    public class EmptyDeveloperPageException : Exception
    {
        /// <param name="developerPageUrl">страница разработчика</param>
        public EmptyDeveloperPageException(string developerPageUrl) :
            base($"На странице {developerPageUrl} нет игр")
        { }
    }
}