namespace C4S.Services.Implements.Parsers.Exceptions
{
    public class InvalidGameIdException : Exception
    {
        public InvalidGameIdException(string gameId)
            : base($"не удалось преобразовать id - {gameId} в int")
        { }
    }
}