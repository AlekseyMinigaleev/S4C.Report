using C4S.DB.DTO;
using C4S.Services.Extensions;
using C4S.Services.Implements.ExcelFileServices;
using C4S.Services.Interfaces;
using FluentValidation;
using MediatR;
using С4S.API.Features.GameStatisticReport.ViewModels;

namespace С4S.API.Features.GameStatisticReport.Actions
{
    public class GetReportFileWithGameStatistics
    {
        public class Query : IRequest<FileViewModel>
        {
            public DateTimeRange DateRange { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.DateRange.FinishDate)
                    .LessThan(x => x.DateRange.StartDate.AddYears(1))
                    .WithMessage(x => $"Нельзя выбрать период больше 1 года");
            }
        }

        public class Handler : IRequestHandler<Query, FileViewModel>
        {
            private readonly DetailedReportService _detailedReportFileService;

            private const string FileName = "S4C.Report.xlsx";
            private const string DetailedStatisticWorksheetName = "Detailed statistics";
            private const string MimeTypeForXLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            public Handler(
               IEnumerable<IExcelWorksheetService> excelFileServices)
            {
                _detailedReportFileService = excelFileServices
                    .Resolve<DetailedReportService>();
            }

            public async Task<FileViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                using var excelPackage = _detailedReportFileService
                    .AddWithNewPackage(DetailedStatisticWorksheetName, request.DateRange);

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