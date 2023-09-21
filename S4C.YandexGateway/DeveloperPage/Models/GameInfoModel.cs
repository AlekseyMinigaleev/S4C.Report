using AutoMapper;
using C4S.DB.Models;
using System.Linq.Expressions;

namespace S4C.YandexGateway.DeveloperPage.Models
{
    /// <summary>
    /// Информация об игре
    /// </summary>
    /// <remarks>
    /// Включает в себя все необходимые поля для <see cref="GameModel"/> и <see cref="GameStatisticModel"/>
    /// </remarks>
    public class GameInfoModel
    {
        /// <summary>
        /// Название игры
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// id игры
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// Дата публикации
        /// </summary>
        public int FirstPublished { get; set; }

        /// <summary>
        /// Оценка
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Количество игроков
        /// </summary>
        public int PlayersCount { get; set; }

        public double? CashIncome { get; set; }

        /// <summary>
        /// Имена всех категорий, к которым относится игра
        /// </summary>
        public string[] CategoriesNames { get; set; }

        public GameInfoModel(
            int appId,
            string title,
            int firstPublished,
            double rating,
            int playersCount,
            string[] categoriesNames)
        {
            AppId = appId;
            Title = title;
            FirstPublished = firstPublished;
            Rating = rating;
            PlayersCount = playersCount;
            CategoriesNames = categoriesNames;
        }
    }

    /// <summary>
    /// Справочник <see cref="Expression"/> для <see cref="GameInfoModel"/>
    /// </summary>
    public static class GameInfoExpression
    {
        /// <summary>
        /// <see cref="Expression"/>, указывающий имеет ли игра статус "новая"
        /// </summary>
        public static Expression<Func<GameInfoModel, bool>> IsNew =
            (gameInfo) => gameInfo.CategoriesNames.Contains("new");

        /// <summary>
        ///  <see cref="Expression"/>, указывающий имеет ли игра статус "продвигаемая"
        /// </summary>
        public static Expression<Func<GameInfoModel, bool>> IsPromoted =
            (gameInfo) => gameInfo.CategoriesNames.Contains("new");
    }

    /// <summary>
    /// Профиль маппинга в <see cref="GameModel"/> из <see cref="GameInfoModel"/>
    /// </summary>
    public class GameModelProfiler : Profile
    {
        public GameModelProfiler()
        {
            CreateMap<GameInfoModel, GameModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AppId))
                .ForMember(dest => dest.PageId, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.FirstPublished).DateTime));

            CreateMap<GameInfoModel, GameStatisticModel>()
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.AppId))
                .ForMember(dest => dest.Evaluation, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.PlayersCount, opt => opt.MapFrom(src => src.PlayersCount))
                .ForMember(dest => dest.PlayersCount, opt => opt.MapFrom(src => src.PlayersCount))
                .ForMember(dest => dest.Statuses, opt => opt.Ignore())
                .ForMember(dest => dest.CashIncome, opt => opt.MapFrom(src => src.CashIncome))
                .ForMember(dest => dest.LastSynchroDate, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}