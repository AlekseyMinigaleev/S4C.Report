namespace C4S.Services.Implements.Parsers.Exceptions
{
    public class EmptyDeveloperPageException: Exception
    {
        public EmptyDeveloperPageException(string developerPageUrl) :
            base($"На странице {developerPageUrl} нет игр")
        { }
    }
}
