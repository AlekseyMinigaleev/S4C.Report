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

        [Display(Name = "получение всех данных по играм")]
        SyncGameInfoAndGameCreateGameStatistic,

        /*TODO: изменить нейминг после уточнения*/

        [Display(Name = "Парсинг денег")]
        ParseGameStatisticFromDENGI
    }
}