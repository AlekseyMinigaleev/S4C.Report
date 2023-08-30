namespace S4C.YandexGateway.DeveloperPageGateway.Exceptions
{
    public class InvalidGameIdException : Exception
    {
        public InvalidGameIdException(string gameId)
            : base($"не удалось преобразовать id - {gameId} в int")
        { }
    }
}
