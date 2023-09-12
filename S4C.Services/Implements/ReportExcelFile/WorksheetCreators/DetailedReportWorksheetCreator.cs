using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Extensions;
using C4S.Helpers.Logger;
using C4S.Helpers.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace C4S.Services.Implements.ReportExcelFile.WorksheetCreators
{
    /// <summary>
    /// Класс ответственный за создание листа с отчетом детальной статистики по играм в указанном excel файле
    /// </summary>
    public class DetailedReportWorksheetCreator : BaseWorksheetCreator
    {
        private readonly ReportDbContext _dbContext;
        private const int GameNameCellLength = 4;

        public DetailedReportWorksheetCreator(
            ReportDbContext dbContext,
            ExcelPackage package,
            string worksheetName,
            BaseLogger logger)
            : base(package, worksheetName, logger)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Создает новый лист, содержащий детальный отчет по статистике всех игр
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task CreateAsync(CancellationToken cancellationToken = default)
        {
            Worksheet = Package.Workbook.Worksheets
              .Add(WorksheetName);
            Logger.LogInformation($"в файл добавлена страница `{WorksheetName}`");

            var games = await _dbContext.Games
                .Include(x => x.GameStatistics)
                    .ThenInclude(x => x.GameGameStatus)
                        .ThenInclude(x => x.GameStatus)
                 .ToArrayAsync(cancellationToken);
            Logger.LogInformation($"Получены данные по всем играм");

            Logger.LogInformation($"Начат процесс записи данных страницы `{WorksheetName}`");
            WriteData(games);
            Logger.LogSuccess($"Процесс записи данных страницы `{WorksheetName}` завершен");
        }

        private void WriteData(IEnumerable<GameModel> games)
        {
            var cell = new ExcelCell(1, 1);
            foreach (var game in games)
            {
                PrintGameName(cell, game);
                Logger.LogInformation($"Записан заголовок для игры '{game.Name}'");

                var statisticColumns = GetStatisticColumns(game.GameStatistics);

                var nextCell = new ExcelCell(
                    cell.Row + 1,
                    cell.Column);

                Logger.LogInformation($"Начата запись игровой статистики игры '{game.Name}'");
                PrintStatisticColumns(nextCell, statisticColumns);
                Logger.LogSuccess($"Игровая статистика игры '{game.Name}' записана");

                cell.Column += GameNameCellLength;
            }
        }

        private void PrintGameName(
           ExcelCell mergedCellStart,
           GameModel game)
        {
            var mergedCellEnd = new ExcelCell(
                mergedCellStart.Row,
                mergedCellStart.Column + GameNameCellLength - 1); //текущая колонка + длина и исключаем текущую колонку
            var value = $"{game.Name} ({game.PublicationDate?.ToString("MM/dd/yyyy")})";

            Worksheet
                .Cells[mergedCellStart.Row, mergedCellStart.Column, mergedCellEnd.Row, mergedCellEnd.Column]
                .Value = value;

            Worksheet
                .Cells[mergedCellStart.Row, mergedCellStart.Column, mergedCellEnd.Row, mergedCellEnd.Column]
                .Merge = true;

            Worksheet
                .Cells[mergedCellStart.Row, mergedCellStart.Column, mergedCellEnd.Row, mergedCellEnd.Column]
                .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private Dictionary<string, IEnumerable<object?>> GetStatisticColumns(
            IEnumerable<GameStatisticModel> gameStatistics)
        {
            var columns = new Dictionary<string, IEnumerable<object?>>
            {
                ["Дата последней синхронизации"] = gameStatistics
                  .Select(x => x.LastSynchroDate.ToString("MM/dd/yyyy")),

                ["Количество игроков"] = gameStatistics
                  .Select(x => x.PlayersCount as object),

                ["Оценка"] = gameStatistics
                  .Select(x => x.Evaluation as object),

                ["Статусы"] = gameStatistics
                .Select(x =>
                    x.Statuses.Count == 0
                        ? "-"
                        : string.Join(", ", x.Statuses))
            };

            return columns;
        }

        private void PrintStatisticColumns(
            ExcelCell cell,
            Dictionary<string, IEnumerable<object?>> columnsData)
        {
            foreach (var columnData in columnsData)
            {
                PrintGameStatisticHeaders(cell, columnData.Key);
                Logger.LogInformation($"Записан заголовок колонки '{columnData.Key}'");

                var nextCell = new ExcelCell(
                    cell.Row + 1,
                    cell.Column);

                PrintGameStatisticValues(nextCell, columnData.Value);
                Logger.LogInformation($"Записаны значения колонки '{columnData.Key}'");

                cell.Column++;
            }
        }

        private void PrintGameStatisticHeaders(
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

        private void PrintGameStatisticValues(
            ExcelCell cell,
            IEnumerable<object?> values)
        {
            foreach (var value in values)
            {
                Worksheet.Cells[cell.Row, cell.Column]
                    .SetCellValueWithAligment(
                        value,
                        ExcelHorizontalAlignment.Center);

                cell.Row++;
            }
        }
    }
}