namespace C4S.Services.Helpers
{
    public static class ParsersHelpers
    {
        public static void ThrowIfNull(object? @object, string message)
        {
            var condition = @object is null;
            ThrowIf(condition, message);
        }

        public static void ThrowIf(bool condition, string message)
        {
            if (condition)
                throw new Exception(message);
        }
    }
}
