namespace S4C.YandexGateway.DeveloperPage.Exceptions
{
    /// <summary>
    /// Ошибка, указывающая на некорректное id игры.
    /// </summary>
    public class InvalidGameIdException : Exception
    {
        /// <param name="gameId">id игры, которое вызвало ошибку</param>
        public InvalidGameIdException(string gameId)
            : base($"не удалось преобразовать id - {gameId} в int")
        { }
    }
}
