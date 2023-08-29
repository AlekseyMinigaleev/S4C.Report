namespace C4S.Services.Implements.Parsers.Exceptions
{
    public class InvalidContractException:Exception
    {
        public string JsonString { get; private set; }
        public string? Key { get; private set; }

        public InvalidContractException(string jsonString, string? key = default) :
            base("Ошибка при десериализации JSON-string, возможно со стороны Yandex, был обновлен контракт.")
        {
            JsonString = jsonString;
            Key = key;
        }
    }
}
