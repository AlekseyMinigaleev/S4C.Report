using C4S.Helpers.Logger;
using OfficeOpenXml;

namespace C4S.Services.Implements.ReportExcelFile.WorksheetCreators
{
    /// <summary>
    /// Базовый класс ответственный за создание листа в excel файле
    /// </summary>
    public class BaseWorksheetCreator
    {
        /// <summary>
        /// <see cref="ExcelPackage"/> в котором будет создан новый лист
        /// </summary>
        protected readonly ExcelPackage Package;

        /// <summary>
        /// Созданный лист
        /// </summary>
        protected ExcelWorksheet Worksheet;

        /// <inheritdoc cref="BaseLogger"/>
        protected readonly BaseLogger Logger;

        /// <summary>
        /// Имя нового листа
        /// </summary>
        protected readonly string WorksheetName;

        public BaseWorksheetCreator(
            ExcelPackage package,
            string worksheetName,
            BaseLogger loggers)
        {
            Package = package;
            Logger = loggers;
            WorksheetName = worksheetName;
        }

        /// <summary>
        /// Создает новый пустой лист в <see cref="Package"/> с именем <see cref="WorksheetName"/>
        /// </summary>
        ///<exception cref="InvalidOperationException"/>
        public virtual async Task CreateAsync(CancellationToken cancellationToken = default)
        {
            Worksheet = Package.Workbook.Worksheets
                .Add(WorksheetName);
        }
    }
}
