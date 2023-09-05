using System.Linq.Expressions;

namespace C4S.DB.Models
{
    /// <summary>
    /// C4S пользователь
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        public Guid Id { get; private set; }

        /*TODO:*/

        #region на эти поля пока нет ограничений, поскольку не понятно в каком виде это должно быть

        public string Login { get; set; }
        public string Password { get; set; }

        #endregion на эти поля пока нет ограничений, поскольку не понятно в каком виде это должно быть

        /// <summary>
        /// Список аккаунтов Яндекс игр
        /// </summary>
        public ISet<YandexGamesAccountModel>? YandexGamesAccounts { get; private set; }

        public UserModel(
            ISet<YandexGamesAccountModel>? yandexGamesAccounts = default)
        {
            Id = Guid.NewGuid();
            YandexGamesAccounts = yandexGamesAccounts;
        }

        private UserModel()
        { }
    }

    /// <summary>
    /// Словарь <see cref="Expression"/> для <see cref="UserModel"/>
    /// </summary>
    public static class UserExpressions
    { }
}