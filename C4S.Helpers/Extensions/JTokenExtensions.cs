using Newtonsoft.Json.Linq;
using S4C.Helpers.Exceptions;

namespace C4S.Helpers.Extensions
{
    public static class JTokenExtensions
    {
        /// <summary>
        /// Получает значение типа <typeparamref name="T"/> из указанного <paramref name="jToken"/> по <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">Тип значения, которое необходимо извлечь.</typeparam>
        /// <param name="key">Ключ, по которому следует извлечь значение из <see cref="JToken"/>.</param>
        /// <param name="jToken"><see cref="JToken"/>, из которого нужно извлечь значение.</param>
        /// <returns>
        /// Извлеченное значение типа <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="InvalidContractException"/>
        public static T GetValue<T>(this JToken jToken,
            string key)
        {
            T value;
            jToken = jToken[key];

            if (jToken is null)
                throw new InvalidContractException(key);

            if (typeof(T).IsArray)
                value = jToken.ToObject<T>();
            else
                value = jToken.Value<T>();

            return value;
        }

    }
}
