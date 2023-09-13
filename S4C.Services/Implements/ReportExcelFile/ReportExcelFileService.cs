using AutoMapper;
using C4S.DB;
using C4S.Helpers.Logger;
using C4S.Services.Implements.ReportExcelFile.WorksheetCreators;
using C4S.Services.Interfaces;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace C4S.Services.Implements.ReportExcelFile
{
    /// <inheritdoc cref="IReportExcelFileService"/>
    public class ReportExcelFileService : IReportExcelFileService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportExcelFileService> _iLogger;

        private BaseLogger _logger;
        private const string DetailedStatisticWorksheetName = "Detailed statistics";

        public ReportExcelFileService(
            ReportDbContext dbContext,
            ILogger<ReportExcelFileService> iLogger,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _iLogger = iLogger;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<byte[]> GetReportAsByteArray(
            CancellationToken cancellationToken)
        {
            _logger = new ConsoleLogger<ReportExcelFileService>(_iLogger);

            byte[] fileByteArray;

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

                fileByteArray = package.GetAsByteArray();
            };
            _logger.LogSuccess($"Файл сохранен");

            return fileByteArray;
        }
    }
}