namespace S4C.Helpers.Exceptions
{
    /// <summary>
    /// Ошибка, указывающая на обновление контракта
    /// </summary>
    public class InvalidContractException : Exception
    {
        /// <summary>
        /// Ключ по которому произошла ошибка
        /// </summary>
        public string Key { get; private set; }

        /// <param name="key">Ключ по которому произошла ошибка</param>
        public InvalidContractException(string key) :
            base($"Ошибка при десериализации ответа. Key = {key}")
        {
            Key = key;
        }
    }
}