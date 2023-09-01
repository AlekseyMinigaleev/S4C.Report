using Hangfire.Console;
using Hangfire.Server;

namespace C4S.Helpers.HangfireHelpers
{
    /// <inheritdoc cref="IHangfireLogger"/>
    public class HangfireLogger : IHangfireLogger
    {
        private readonly PerformContext _hangfireContext;

        public HangfireLogger(PerformContext hangfireContext)
        {
            _hangfireContext = hangfireContext;
        }

        /// <inheritdoc/>
        public void LogError(string message)
        {
            Log(message, HangfireLogLevel.Error);
        }

        /// <inheritdoc/>
        public void LogInformation(string message)
        {
            Log(message, HangfireLogLevel.Information);
        }

        /// <inheritdoc/>
        public void LogSuccess(string message)
        {
            Log(message, HangfireLogLevel.Success);
        }

        /// <inheritdoc/>
        public void LogWarning(string message)
        {
            Log(message, HangfireLogLevel.Warning);
        }

        /// <inheritdoc/>
        public void Log(string message, HangfireLogLevel logLevel)
        {
            var color = logLevel switch
            {
                HangfireLogLevel.Success => ConsoleTextColor.Green,
                HangfireLogLevel.Warning => ConsoleTextColor.Yellow,
                HangfireLogLevel.Error => ConsoleTextColor.Red,
                HangfireLogLevel.Information => ConsoleTextColor.White,
                _ => ConsoleTextColor.White,
            };

            _hangfireContext.SetTextColor(color);
            _hangfireContext.WriteLine(message);
            _hangfireContext.ResetTextColor();
        }
    }
}