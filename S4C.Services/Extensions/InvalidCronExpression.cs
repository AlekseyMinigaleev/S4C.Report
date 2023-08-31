namespace C4S.Services.Extensions
{
    public class InvalidCronExpression : Exception
    {
        public readonly string? CronExpression;

        public InvalidCronExpression(string? cronExpression) : base($"Указанное выражение cron не является валидным")
        {
            CronExpression = cronExpression;
        }
    }
}