using Hangfire;

namespace C4S.DB.Models.Hangfire
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
            AddOrUpdate(new HangfireJobConfigurationModel
            {
                JopType = jobType,
                CronExpression = cronExpression,
                IsEnable = isEnable
            });
        }

        public void Update(HangfireJobConfigurationModel hangfireJobConfiguration)
        {
            AddOrUpdate(hangfireJobConfiguration);
        }

        private void AddOrUpdate(HangfireJobConfigurationModel hangfireJobConfiguration)
        {
            JopType = hangfireJobConfiguration.JopType;
            CronExpression = hangfireJobConfiguration.CronExpression;
            IsEnable = CronExpression is null
                ? false
                : hangfireJobConfiguration.IsEnable;
        }
    }

    public static class HangfireJobConfigurationConstants
    {
        public static readonly string DefaultCronExpression = Cron.Never().ToString(); /*TODO: уточнить*/
        public const bool DefaultIsEnable = false; /*TODO: уточнить*/
    }
}