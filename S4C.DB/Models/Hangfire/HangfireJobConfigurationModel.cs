using Hangfire;

namespace C4S.DB.Models.Hangfire
{
    public class HangfireJobConfigurationModel
    {
        public HangfireJobTypeEnum JobType { get; private set; }

        public string? CronExpression { get; private set; }

        public bool IsEnable { get; private set; }

        private HangfireJobConfigurationModel()
        { }

        public HangfireJobConfigurationModel(
            HangfireJobTypeEnum jobType,
            string? cronExpression,
            bool isEnable)
        {
            JobType = jobType;
            CronExpression = cronExpression;
            SetIsEnable(isEnable);
        }

        public void Update(string? cronExpression, bool isEnable)
        {
            CronExpression = cronExpression;
            SetIsEnable(isEnable);
        }

        public void SetIsEnable(bool isEnable) =>
            IsEnable = CronExpression is null
                ? false
                : isEnable;
    }

    public static class HangfireJobConfigurationConstants
    {
        public static readonly string DefaultCronExpression = Cron.Never().ToString(); /*TODO: уточнить*/
        public const bool DefaultIsEnable = false; /*TODO: уточнить*/
    }
}