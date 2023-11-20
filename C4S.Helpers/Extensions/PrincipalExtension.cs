using System.Security.Claims;
using System.Security.Principal;

namespace C4S.Helpers.Extensions
{
    /// <summary>
    /// Статический класс с методами-расширениями для интерфейса IPrincipal.
    /// </summary>
    public static class PrincipalExtension
    {
        /// <summary>
        /// Получает идентификатор пользователя из объекта IPrincipal.
        /// </summary>
        /// <param name="principal">Объект IPrincipal, представляющий текущего пользователя.</param>
        /// <exception cref="InvalidOperationException"/>
        public static Guid GetUserId(
            this IPrincipal principal)
        {
            var claimsIdentity = principal.Identity as ClaimsIdentity;

            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                return userId;

            throw new InvalidOperationException(
                $"Не удалось получить или преобразовать идентификатор пользователя из клейма." +
                $" Значение клейма идентификатора пользователя: {userIdClaim}");
        }
    }
}
