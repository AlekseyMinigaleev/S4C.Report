namespace C4S.Helpers.HangfireHelpers
{
	/// <summary>
	/// Перечисление всех возможных уровней логирования, логгера <see cref="IHangfireLogger"/>
	/// </summary>
	public enum HangfireLogLevel
	{
		/// <summary>
		/// Уровень логирование при успешном результате
		/// </summary>
        /// <remarks>
        /// При логировании на этом уровне текст сообщения будет зеленым.
        /// </remarks>
		Success,

        /// <summary>
        /// Уровень логирование при предупреждении
        /// </summary>
        /// <remarks>
        /// При логировании на этом уровне текст сообщения будет желтым.
        /// </remarks>
        Warning,

        /// <summary>
        /// Уровень логирование при ошибке
        /// </summary>
        /// <remarks>
        /// При логировании на этом уровне текст сообщения будет красным.
        /// </remarks>
        Error,

        /// <summary>
        /// базовый уровень логирования
        /// </summary>
        /// <remarks>
        /// При логировании на этом уровне текст сообщения будет белым.
        /// </remarks>
        Information
    }
}
