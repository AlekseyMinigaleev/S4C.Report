using Hangfire;
using System.Linq.Expressions;

namespace C4S.DB.Models.Hangfire
{
    /// <summary>
    /// Сущность конфигурации джобы
    /// </summary>
    public class HangfireJobConfigurationModel
    {
        /// <summary>
        /// Тип джобы
        /// </summary>
        public HangfireJobTypeEnum JobType { get; private set; }

        /// <summary>
        /// cron выражение
        /// </summary>
        public string? CronExpression { get; private set; }

        /// <summary>
        /// Статус джобы
        /// </summary>
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

        /// <summary>
        /// Выполняет обновление <see cref="HangfireJobConfigurationModel"/>
        /// </summary>
        /// <param name="cronExpression">cron выражение</param>
        /// <param name="isEnable">статус джобы</param>
        public void Update(string? cronExpression, bool isEnable)
        {
            CronExpression = cronExpression;
            SetIsEnable(isEnable);
        }

        /// <summary>
        /// Изменяет статус джобы на указанный
        /// </summary>
        /// <param name="isEnable">новый статус джобы</param>
        public void SetIsEnable(bool isEnable) =>
            IsEnable = !string.IsNullOrWhiteSpace(CronExpression) && isEnable;

        /// <summary>
        /// Нормализует крон выражение
        /// </summary>
        public void NormalizeCronExpression()
        {
            //потому что мы поддерживаем CronExpression = string.Empty, а hangfire нет
            if (string.IsNullOrWhiteSpace(CronExpression))
                Update(null, IsEnable);
        }
    }

    /// <summary>
    /// Справочник <see cref="Expression"/> для <see cref="HangfireJobConfigurationModel"/>
    /// </summary>
    public static class HangfireJobConfigurationExpression
    { }

    /// <summary>
    /// Справочник констант для <see cref="HangfireJobConfigurationModel"/>
    /// </summary>
    public static class HangfireJobConfigurationConstants
    {
        public static readonly string DefaultCronExpression = Cron.Never().ToString(); /*TODO: уточнить*/

        public const bool DefaultIsEnable = false; /*TODO: уточнить*/
    }
}