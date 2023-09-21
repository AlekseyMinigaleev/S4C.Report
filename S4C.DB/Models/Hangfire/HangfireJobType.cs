using System.ComponentModel.DataAnnotations;

namespace C4S.DB.Models.Hangfire
{
    /// <summary>
    /// Перечисление всех возможных типов джоб
    /// </summary>
    public enum HangfireJobType
    {
        [Display(Name = "Парсинг id игр со страницы разработчика")]
        ParseGameIdsFromDeveloperPage,

        [Display(Name = "Получение всех данных по играм")]
        SyncGameInfoAndGameCreateGameStatistic,
    }
}