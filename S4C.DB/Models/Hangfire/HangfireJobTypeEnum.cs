using System.ComponentModel.DataAnnotations;

namespace C4S.DB.Models.Hangfire
{
	public enum HangfireJobTypeEnum
	{
		[Display(Name = "Парсинг id игр со страницы разработчика")]
		ParseGameIdsFromDeveloperPage,

		[Display(Name = "Парсинг игровой статистики со страницы разработчика")]
		ParseGameStatisticFromDeveloperPage,

		/*TODO: изменить нейминг после уточнения*/
		[Display(Name = "Парсинг денег")]
		ParseGameStatisticFromDENGI
	}
}
