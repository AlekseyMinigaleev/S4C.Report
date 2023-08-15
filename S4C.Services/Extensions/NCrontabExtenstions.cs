using NCrontab;

namespace C4S.Services.Extensions
{
    public static class NCrontabExtenstions
    {
        public static bool TryParseWithNullOrEmpty(string cronExpression, out CrontabSchedule? result )
        {
            bool boolResult;
            if (string.IsNullOrWhiteSpace(cronExpression))
            {
                boolResult = true;
                result = null;
            }
            else
            {
                result = CrontabSchedule.TryParse(cronExpression);
                boolResult = result is not null;
            }

            return boolResult;
        }
    }
}
