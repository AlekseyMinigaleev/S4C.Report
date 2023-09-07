using Microsoft.Extensions.Logging;

namespace C4S.Helpers.Logger
{
    /// <summary>
    /// Логгер в консоль
    /// </summary>
    public class ConsoleLogger<T> : BaseLogger
        where T : class
    {
        private readonly ILogger<T> _logger;

        public ConsoleLogger(ILogger<T> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public override void Log(string message, LogLevel logLevel)
        {
            var microsoftLogLevel = logLevel switch
            {
                LogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,/*TODO: проверить*/
                LogLevel.Success => Microsoft.Extensions.Logging.LogLevel.Information, /*TODO: проверить*/
                LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
                LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            };

            _logger.Log(microsoftLogLevel, message);
        }

        /// <inheritdoc/>
        public override void LogError(string message)
        {
            _logger.LogError(message);
        }

        /*TODO: проверить*/

        /// <inheritdoc/>
        public override void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        /*TODO: проверить*/

        /// <inheritdoc/>
        public override void LogSuccess(string message)
        {
            _logger.LogInformation(message);
        }

        /// <inheritdoc/>
        public override void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }
    }
}