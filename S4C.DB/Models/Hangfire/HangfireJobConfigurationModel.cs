namespace S4C.DB.Models.Hangfire
{
    public class HangfireJobConfigurationModel
    {
        public HangfireJobTypeEnum JopType { get; set; }

        public string? CronExpression { get; set; }

        public bool IsEnabled { get; set; }
    }
}
