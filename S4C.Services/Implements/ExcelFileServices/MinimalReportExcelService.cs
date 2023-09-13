
using C4S.Services.Interfaces;
using OfficeOpenXml;

namespace C4S.Services.Implements.ExcelFileServices
{
    //TODO:
    public class MinimalReportExcelService : IExcelFileService
    {
        public ExcelWorksheet AddWorksheet(ExcelPackage package, string fileName)
        {
            var resutl = package.Workbook.Worksheets
                .Add(fileName);

            return resutl;
        }

        public async Task<byte[]> CreateNewFileAsync(string fileName, CancellationToken cancellationToken)
        {
            using var package = new ExcelPackage();
            AddWorksheet(package, fileName);
            var bytes = await package.GetAsByteArrayAsync(cancellationToken);
            return bytes;
        }
    }
}
