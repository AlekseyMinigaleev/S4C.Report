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
            private readonly IReportExcelFileService _reportExcelFileService;
            private const string FileName = "S4C.Report.xlsx";
            private const string MimeTypeForXLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            public Handler(IReportExcelFileService reportExcelFileService)
            {
                _reportExcelFileService = reportExcelFileService;
            }

            public async Task<FileViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                var fileBytes = await _reportExcelFileService.GetReportFile(cancellationToken);

                var file = new FileViewModel(
                    fileBytes,
                    FileName,
                    MimeTypeForXLSX);

                return file;
            }
        }
    }
}