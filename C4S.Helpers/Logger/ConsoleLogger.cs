using Microsoft.Extensions.Logging;

namespace C4S.Helpers.Logger
{
    public class ConsoleLogger<T> : BaseLogger
        where T : class
    {
        private readonly ILogger<T> _logger;

        public ConsoleLogger(ILogger<T> logger)
        {
            _logger = logger;
        }

        public override void Log(string message, LogLevel logLevel)
        {
            var microsofrLogLevel = logLevel switch
            {
                LogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,/*TODO: проверить*/
                LogLevel.Success => Microsoft.Extensions.Logging.LogLevel.Information, /*TODO: проверить*/
                LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
                LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            };

            _logger.Log(microsofrLogLevel, message);
        }

        public override void LogError(string message)
        {
            _logger.LogError(message);
        }

        /*TODO: проверить*/
        public override void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        /*TODO: проверить*/
        public override void LogSuccess(string message)
        {
            _logger.LogInformation(message);
        }

        public override void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }
    }
}
