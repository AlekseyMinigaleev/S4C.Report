using System.ComponentModel.DataAnnotations;

namespace C4S.DB.Models.Hangfire
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
