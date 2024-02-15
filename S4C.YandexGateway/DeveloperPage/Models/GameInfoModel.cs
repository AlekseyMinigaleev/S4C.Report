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
        /// <inheritdoc cref="GameModel.Name"/>
        public string Title { get; set; }

        /// <inheritdoc cref="GameModel.AppId"/>
        public int AppId { get; set; }

        /// <inheritdoc cref="GameModel.PublicationDate"/>
        public int FirstPublished { get; set; }

        /// <inheritdoc cref="GameStatisticModel.Evaluation"/>
        public double Evaluation { get; set; }

        /// <inheritdoc cref="GameStatisticModel.PlayersCount"/>
        public int PlayersCount { get; set; }

        /// <inheritdoc cref="GameStatisticModel.CashIncome"/>
        public double? CashIncome { get; set; }

        /// <inheritdoc cref="GameStatisticModel.Rating"/>
        public int? Rating { get; set; }

        /// <inheritdoc cref="GameModel.PreviewURL"/>
        public string PreviewURL { get; set; }

        /// <summary>
        /// Имена всех категорий, к которым относится игра
        /// </summary>
        public string[] CategoriesNames { get; set; }

        public GameInfoModel(
            int appId,
            string title,
            int firstPublished,
            double evaluation,
            int playersCount,
            string[] categoriesNames,
            string previewURL,
            int? rating = default,
            double? cashIncome = default)
        {
            AppId = appId;
            Title = title;
            FirstPublished = firstPublished;
            Evaluation = evaluation;
            PlayersCount = playersCount;
            CategoriesNames = categoriesNames;
            PreviewURL = previewURL;
            CashIncome = cashIncome;
            Rating = rating;
        }

        private GameInfoModel()
        { }
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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.FirstPublished).DateTime))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AppId, opt => opt.Ignore())
                .ForMember(dest => dest.PageId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.URL, opt => opt.Ignore())
                .ForMember(dest => dest.GameStatistics, opt => opt.Ignore());

            CreateMap<GameInfoModel, GameStatisticModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.LastSynchroDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Statuses, opt => opt.Ignore())
                .ForMember(dest => dest.GameGameStatus, opt => opt.Ignore());
        }
    }
}