using AutoMapper;
using C4S.DB;
using C4S.Helpers.Logger;
using C4S.Services.Implements.ReportExcelFile.WorksheetCreators;
using C4S.Services.Interfaces;
using Hangfire.Server;
using OfficeOpenXml;

namespace C4S.Services.Implements.ReportExcelFile
{
    /// <inheritdoc cref="IReportExcelFileService"/>
    public class ReportExcelFileService : IReportExcelFileService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IMapper _mapper;
        private BaseLogger _logger;
        private const string DetailedStatisticWorksheetName = "Detailed statistics";

        public ReportExcelFileService(
            ReportDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task CreateFileAsync(
            PerformContext hangfireContext,
            CancellationToken cancellationToken)
        {
            _logger = new HangfireLogger(hangfireContext);

            /*TODO: вынести в YandexGameAccount и переименовать таблицу на YandexGameAccountSettings*/
            var excelPath = @"D:\папка\C4S\S4C.Report.xlsx";
            var fileInfo = new FileInfo(excelPath);

            _logger.LogInformation($"Начат процесс создания файла");
            if (fileInfo.Exists)
            {
                _logger.LogInformation($"Файл уже содержится и будет пересоздан");
                fileInfo.Delete();
                _logger.LogInformation($"Файл удален");
            }

            using var file = fileInfo.Create();
            _logger.LogInformation($"Создан новый файл");
            using (var package = new ExcelPackage())
            {
                var detailedWorksheetCreator = new DetailedReportWorksheetCreator(
                    _dbContext,
                    _mapper,
                    package,
                    DetailedStatisticWorksheetName,
                    _logger);

                await detailedWorksheetCreator.CreateAsync(
                    cancellationToken);

                package.SaveAs(file);
            };
            _logger.LogSuccess($"Файл сохранен");
        }
    }
}