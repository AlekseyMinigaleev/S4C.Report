namespace C4S.Services.Exceptions
{
    public class HangfireDatabaseConnectException : Exception 
    {
        public string? ConnectionString { get; set; }

        public HangfireDatabaseConnectException(string connectionString)
            : base($"Не удалось подключиться к базе данных hangfire. ConnectionString: {connectionString}")
        {
            ConnectionString = connectionString;
        }

        public HangfireDatabaseConnectException()
            : base($"Не удалось подключиться к базе данных hangfire.")
        { }
    }
}
