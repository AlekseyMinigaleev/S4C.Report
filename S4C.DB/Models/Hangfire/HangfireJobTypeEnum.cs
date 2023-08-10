using System.ComponentModel.DataAnnotations;

namespace S4C.DB.Models.Hangfire
{
	public enum HangfireJobTypeEnum
	{
		[Display(Name = "Парсинг игровой статистики со страницы разработчика")]
		ParseGameStatisticFromDeveloperPage,

		/*TODO: изменить нейминг после уточнения*/
		[Display(Name = "Парсинг денег")]
		ParseGameStatisticFromDENGI
	}
}
