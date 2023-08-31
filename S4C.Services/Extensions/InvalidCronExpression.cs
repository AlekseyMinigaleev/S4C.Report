namespace C4S.Services.Extensions
{
    /// <summary>
    /// Ошибка, обозначающая некорректное cron выражение.
    /// </summary>
    public class InvalidCronExpression : Exception
    {
        /// <summary>
        /// Cron выражение, которое вызвало ошибку.
        /// </summary>
        public readonly string? CronExpression;

        /// <param name="cronExpression"> Cron выражение, которое вызвало ошибку.</param>
        public InvalidCronExpression(string? cronExpression) : base($"Указанное выражение cron не является валидным")
        {
            CronExpression = cronExpression;
        }
    }
}