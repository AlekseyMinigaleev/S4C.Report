using C4S.Services.Implements.ExcelFileServices;
using C4S.Services.Interfaces;
using MediatR;
using С4S.API.Features.GameStatisticReport.ViewModels;

namespace С4S.API.Features.GameStatisticReport.Actions
{
    public class GetGameStatisticReportFile
    {
        public class Query : IRequest<FileViewModel>
        { }

        public class Handler : IRequestHandler<Query, FileViewModel>
        {
            private readonly DetailedReportExcelService _detailedReportFileService;

            private const string FileName = "S4C.Report.xlsx";
            private const string DetailedStatisticWorksheetName = "Detailed statistics";
            private const string MimeTypeForXLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            public Handler(
               IEnumerable<IExcelFileService> excelFileServices)
            {
                _detailedReportFileService = excelFileServices
                    .Resolve<DetailedReportExcelService>();
            }

            public async Task<FileViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                using var excelPackage = _detailedReportFileService
                    .CreateNewExcelPackage(DetailedStatisticWorksheetName);

                var fileBytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);

                var file = new FileViewModel(
                    fileBytes,
                    FileName,
                    MimeTypeForXLSX);

                return file;
            }
        }
    }
}