namespace C4S.Services.Exceptions
{
    /// <summary>
    /// Ошибка, обозначающая некорректное cron выражение.
    /// </summary>
    public class InvalidCronExpressionException : Exception
    {
        /// <summary>
        /// Cron выражение, которое вызвало ошибку.
        /// </summary>
        public readonly string? CronExpression;

        /// <param name="cronExpression"> Cron выражение, которое вызвало ошибку.</param>
        public InvalidCronExpressionException(string? cronExpression) : base($"Указанное выражение cron не является валидным")
        {
            CronExpression = cronExpression;
        }
    }
}