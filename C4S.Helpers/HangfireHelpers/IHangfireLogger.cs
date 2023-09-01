namespace C4S.Helpers.HangfireHelpers
{
    /// <summary>
    /// Логгер в консоль Hangfire
    /// </summary>
    public interface IHangfireLogger
    {
        /// <summary>
        /// Выполняет логирование на уровне <see cref="HangfireLogLevel.Information"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        public void LogInformation(string message);

        /// <summary>
        /// Выполняет логирование на уровне <see cref="HangfireLogLevel.Success"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        public void LogSuccess(string message);

        /// <summary>
        /// Выполняет логирование на уровне <see cref="HangfireLogLevel.Warning"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        public void LogWarning(string message);

        /// <summary>
        /// Выполняет логирование на уровне <see cref="HangfireLogLevel.Error"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        public void LogError(string message);

        /// <summary>
        /// Выполняет логирование на уровне <paramref name="logLevel"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        /// <param name="logLevel">Уровень логирования.</param>
        public void Log(string message, HangfireLogLevel logLevel);
    }
}
