using C4S.Services.Implements.ExcelFileServices;
using C4S.Services.Interfaces;
using OfficeOpenXml;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IReportExcelFileService"/>
    public class ReportExcelFileService : IReportExcelFileService
    {
        private readonly DetailedReportExcelService _detailedReportFileService;
        private readonly MinimalReportExcelService _minimalReportFileService;

        private const string DetailedStatisticWorksheetName = "Detailed statistics";
        private const string MinimalStatisticWorksheetName = "Minimal statistics";


        public ReportExcelFileService(
            DetailedReportExcelService detailedReportFileService,
            MinimalReportExcelService minimalReportEcelService)
        {
            _detailedReportFileService = detailedReportFileService;
            _minimalReportFileService = minimalReportEcelService;
        }

        /// <inheritdoc/>
        public async Task<byte[]> GetReportFile(
            CancellationToken cancellationToken)
        {
            byte[] fileByteArray;

            using (var package = new ExcelPackage())
            {
                _detailedReportFileService.AddWorksheet(
                    package,
                    DetailedStatisticWorksheetName);

                _minimalReportFileService.AddWorksheet(
                    package,
                    MinimalStatisticWorksheetName);

                fileByteArray = await package.GetAsByteArrayAsync(cancellationToken);
            };

            return fileByteArray;
        }
    }
}