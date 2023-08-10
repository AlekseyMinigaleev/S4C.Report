namespace S4C.DB.Models.Hangfire
{
    public class HangfireJobConfigurationModel
    {
        public HangfireJobTypeEnum JopType { get; private set; }

        public string? CronExpression { get; private set; }

        public bool IsEnable { get; private set; }

        private HangfireJobConfigurationModel()
        { }

        public HangfireJobConfigurationModel(
            HangfireJobTypeEnum jobType,
            string? cronExpression,
            bool isEnable)
        {
            JopType = jobType;
            CronExpression = cronExpression;
            IsEnable = isEnable;
        }
    }
}
