using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.DTO;
using C4S.DB.Models;
using C4S.Helpers.Extensions;
using C4S.Helpers.Models;
using C4S.Services.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace C4S.Services.Implements.ExcelFileServices
{
    /// <inheritdoc cref="IExcelWorksheetService"/>
    public class DetailedReportService : IExcelWorksheetService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IMapper _mapper;
        private ExcelWorksheet Worksheet;

        private const int GameNameCellLength = 5;

        public DetailedReportService(
            ReportDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public ExcelWorksheet Add(
            ExcelPackage package,
            string worksheetName,
            DateTimeRange dateTimeRange)
        {
            Worksheet = package.Workbook.Worksheets
              .Add(worksheetName);

            var gameViewModelQuery = _dbContext.Games
                .Where(x => x.GameStatistics
                    .Any(gs => dateTimeRange.StartDate.Date <= gs.LastSynchroDate.Date
                         && dateTimeRange.FinishDate.Date >= gs.LastSynchroDate.Date))
                .ProjectTo<GameViewModel>(_mapper.ConfigurationProvider);

            WriteData(gameViewModelQuery);
         
            return Worksheet;
        }

        /// <inheritdoc/>
        public ExcelPackage AddWithNewPackage(
            string worksheetName,
            DateTimeRange dateTimeRange)
        {
            var package = new ExcelPackage();
            Add(package, worksheetName, dateTimeRange);

            return package;
        }

        private void WriteData(
            IQueryable<GameViewModel> gameViewModelQuery)
        {
            var cell = new ExcelCell(1, 1);
            foreach (var game in gameViewModelQuery)
            {
                PrintGameName(cell, game.Name);

                var nextCell = new ExcelCell(
                    cell.Row + 1,
                    cell.Column);

                PrintGameStatisticColumns(game.GameStatistics, nextCell);

                cell.Column += GameNameCellLength;
            }
        }

        private void PrintGameName(
            ExcelCell mergedCellStart,
            string gameName)
        {
            var mergedCellEnd = new ExcelCell(
                mergedCellStart.Row,
                mergedCellStart.Column + GameNameCellLength - 1); //текущая колонка + длина и исключаем текущую колонку

            var excelRange = Worksheet
                .Cells[mergedCellStart.Row, mergedCellStart.Column, mergedCellEnd.Row, mergedCellEnd.Column];

            excelRange.SetCellValueWithAligment(gameName, ExcelHorizontalAlignment.Center);
            excelRange.Merge = true;
        }

        private void PrintGameStatisticColumns(
            IEnumerable<GameStatisticViewModel> gameStatistic,
            ExcelCell cell)
        {
            PrintColumns(
                "Дата синхронизации",
                gameStatistic.Select(x => x.SynchroDate),
                cell);
            cell.Column++;

            PrintColumnsWithCellState(
                "Количество игроков",
                gameStatistic.Select(x => x.PlayersCount).ToArray(),
                cell);
            cell.Column++;

            PrintColumnsWithCellState(
                "Оценка",
                gameStatistic.Select(x => x.Evaluation).ToArray(),
                cell);
            cell.Column++;

            PrintColumns(
                "Статусы",
                gameStatistic.Select(x => x.Statuses),
                cell);
            cell.Column++;

            PrintColumnsWithCellState(
                "Прибыль",
                gameStatistic.Select(x => x.CashIncome).ToArray(),
                cell);
            cell.Column++;
        }

        private void PrintColumns<T>(
            string columnHeader,
            IEnumerable<T> values,
            ExcelCell cell)
        {
            PrintGameStatisticHeader(cell, columnHeader);
            cell.Row++;

            foreach (var value in values)
            {
                Worksheet.Cells[cell.Row, cell.Column]
                   .SetCellValueWithAligment(
                       value,
                       ExcelHorizontalAlignment.Center);
                cell.Row++;
            }
        }

        private void PrintColumnsWithCellState<T>(
            string columnHeader,
            T[] values,
            ExcelCell cell)
            where T : IComparable
        {
            PrintGameStatisticHeader(cell, columnHeader);
            cell.Row++;

            for (int i = 0; i < values.Length; i++)
            {
                Worksheet.Cells[cell.Row, cell.Column]
                  .SetCellValueWithAligment(
                      values[i],
                      ExcelHorizontalAlignment.Center);

                if (i != 0)
                {
                    var result = values[i].CompareTo(values[i - 1]);
                    if (result > 0)
                        Worksheet.Cells[cell.Row, cell.Column].Style.Fill
                            .SetBackground(Color.Green);
                    if (result < 0)
                        Worksheet.Cells[cell.Row, cell.Column].Style.Fill
                            .SetBackground(Color.Red);
                }

                cell.Row++;
            }
        }

        private void PrintGameStatisticHeader(
            ExcelCell cell,
            string headerName)
        {
            Worksheet.Cells[cell.Row, cell.Column]
                .SetCellValueWithAligment(
                    headerName,
                    ExcelHorizontalAlignment.Center);

            Worksheet.Cells[cell.Row, cell.Column]
                .AutoFitColumns();
        }
    }

    public class GameViewModel
    {
        public string Name { get; set; }
        public GameStatisticViewModel[] GameStatistics { get; set; }
    }

    public class GameStatisticViewModel
    {
        public string SynchroDate { get; set; }

        public int PlayersCount { get; set; }

        public double Evaluation { get; set; }

        public string Statuses { get; set; }

        public decimal CashIncome { get; set; }
    }

    public class GameViewModelProfiler : Profile
    {
        public GameViewModelProfiler()
        {
            CreateMap<GameModel, GameViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Name} ({src.PublicationDate.Value.ToString("MM.dd.yyyy")})"))
                .ForMember(dest => dest.GameStatistics, opt => opt.MapFrom(src => src.GameStatistics));

            CreateMap<GameStatisticModel, GameStatisticViewModel>()
                .ForMember(dest => dest.SynchroDate, opt => opt.MapFrom(src => src.LastSynchroDate.ToString("MM.dd.yyyy")))
                .ForMember(dest => dest.PlayersCount, opt => opt.MapFrom(src => src.PlayersCount))
                .ForMember(dest => dest.Evaluation, opt => opt.MapFrom(src => src.Evaluation))
                .ForMember(dest => dest.Statuses, opt => opt.MapFrom(GameStatisticExpression.GetStatusesAsStringExpression))
                .ForMember(dest => dest.CashIncome, opt => opt.MapFrom(src => 0M));
        }
    }
}