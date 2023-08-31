namespace S4C.YandexGateway.DeveloperPageGateway.Exceptions
{
    /// <summary>
    /// Ошибка, указывающая на обновление контракта
    /// </summary>
    public class InvalidContractException : Exception
    {
        /// <summary>
        /// Объект возвращаемый яндексом
        /// </summary>
        public string JsonString { get; private set; }
        /// <summary>
        /// Ключ по которому произошла ошибка
        /// </summary>
        public string? Key { get; private set; }

        /// <param name="jsonString">Объект возвращаемый яндексом</param>
        /// <param name="key">Ключ по которому произошла ошибка</param>
        public InvalidContractException(string jsonString, string? key = default) :
            base("Ошибка при десериализации JSON-string, возможно со стороны Yandex, был обновлен контракт.")
        {
            JsonString = jsonString;
            Key = key;
        }
    }
}
