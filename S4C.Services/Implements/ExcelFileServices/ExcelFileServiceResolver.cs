using C4S.Services.Interfaces;

namespace C4S.Services.Implements.ExcelFileServices
{
    public static class ExcelFileServiceResolver
    {
        /// <summary>
        /// Get instance of type <typeparamref name="T"/> from <paramref name="services"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns>
        /// <typeparamref name="T"/>
        /// </returns>
        /// <exception cref="InvalidCastException"></exception>
        public static T Resolve<T>(this IEnumerable<IExcelFileService> services)
            where T : IExcelFileService
        {
             var service = services
                .Single(x => x.GetType().Name == typeof(T).Name);

            var instance = (T)service;

            return instance is null 
                ? throw new InvalidCastException()
                : instance;
        }
    }
}
