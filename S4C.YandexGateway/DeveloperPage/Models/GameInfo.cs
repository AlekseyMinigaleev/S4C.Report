using AutoMapper;
using C4S.DB.Models;

namespace S4C.YandexGateway.DeveloperPageGateway.Models
{
    /// <summary>
    /// Информация об игре
    /// </summary>
    /// <remarks>
    /// Включает в себя все необходимые поля для <see cref="GameModel"/> и <see cref="GameStatisticModel"/>
    /// </remarks>
    public class GameInfo
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

        /// <summary>
        /// Имена всех категорий, к которым относится игра
        /// </summary>
        public string[] CategoriesNames { get; set; }

        public GameInfo(
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

    public class GameModelProfiler : Profile
    {
        public GameModelProfiler()
        {
            CreateMap<GameInfo, GameModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AppId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.FirstPublished).DateTime));

            CreateMap<GameInfo, GameStatisticModel>()
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.AppId))
                .ForMember(dest => dest.Evaluation, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.PlayersCount, opt => opt.MapFrom(src => src.PlayersCount))
                .ForMember(dest => dest.PlayersCount, opt => opt.MapFrom(src => src.PlayersCount))
                /*TODO: вынести в спеку*/
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.CategoriesNames.Contains("new")))
                /*TODO: вынести в спеку*/
                .ForMember(dest => dest.IsPromoted, opt => opt.MapFrom(src => src.CategoriesNames.Contains("promoted")))
                .ForMember(dest => dest.LastSynchroDate, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}