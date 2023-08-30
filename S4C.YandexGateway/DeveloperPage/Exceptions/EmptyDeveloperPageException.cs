namespace S4C.YandexGateway.DeveloperPageGateway.Exceptions
{
    public class EmptyDeveloperPageException : Exception
    {
        public EmptyDeveloperPageException(string developerPageUrl) :
            base($"На странице {developerPageUrl} нет игр")
        { }
    }}
