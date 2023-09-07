using C4S.Db.Exceptions;
using FluentValidation.Results;
using Hangfire;
using Microsoft.IdentityModel.Tokens;
using NCrontab;
using System.Linq.Expressions;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

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
        public HangfireJobType JobType { get; private set; }

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
            HangfireJobType jobType,
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
            var validationResult = IsValidCronExpression(CronExpression);

            if (!validationResult.IsValid)
                throw new InvalidCronExpressionException(CronExpression);

            //потому что мы поддерживаем CronExpression = string.Empty, а hangfire нет
            if (string.IsNullOrWhiteSpace(CronExpression))
                Update(null, IsEnable);

            CronExpression = cronExpression;
            SetIsEnable(isEnable);
        }

        /// <summary>
        /// Изменяет статус джобы на указанный
        /// </summary>
        /// <param name="isEnable">новый статус джобы</param>
        public void SetIsEnable(bool isEnable) =>
            IsEnable = !string.IsNullOrWhiteSpace(CronExpression) && isEnable;

        //TODO: уточнить, сейчас пользователь может оставить пустое значение для CronExpression и в таком случае IsEnable = false,
        //т.е.пользователь не должен иметь возможности седлать IsEnable = false и CronExpression = string.Empty?
        /// <summary>
        /// Проверяет CronExpression на валидность.
        /// </summary>
        /// <remarks>
        /// <see cref="string.IsNullOrWhiteSpace(string?)"/> является валидным значением
        /// </remarks>
        /// <returns></returns>
        public static ValidationResult IsValidCronExpression(string? cronExpression)
        {
            /*TODO: проверить случай с cronExpression = string.Empty*/
            var crontabSchedule = CrontabSchedule.TryParse(cronExpression);
            var result = new ValidationResult();

            if (crontabSchedule is null && !cronExpression.IsNullOrEmpty())
            {
                var errors = new List<ValidationFailure>
                {
                    new ValidationFailure(nameof(cronExpression),InvalidCronExpressionException.ErrorMessage),
                };

                result = new ValidationResult(errors);
            }

            return result;
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