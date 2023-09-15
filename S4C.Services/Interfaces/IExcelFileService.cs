using OfficeOpenXml;

namespace C4S.Services.Interfaces
{
    /// <summary>
    /// Сервис excel файлов
    /// </summary>
    public interface IExcelFileService
    {
        /// <summary>
        /// Создает новый лист с отчетом в <paramref name="package"/> и именем <paramref name="worksheetName"/>/>
        /// </summary>
        /// <param name="package"><see cref="ExcelPackage"/>, в котором будет создан новый лист</param>
        /// <param name="worksheetName">имя с которым будет создан новый лист</param>
        ///<exception cref="InvalidOperationException"/>
        public ExcelWorksheet AddWorksheet(
            ExcelPackage package,
            string worksheetName);

        /// <summary>
        /// Создает новый <see cref="ExcelPackage"/>, содержащий лист с отчетом
        /// </summary>
        /// <param name="worksheetName"> имя с которым будет создан новый лист</param>
        /// <returns>
        /// <see cref="ExcelPackage"/>[], представляющий содержимое excel файла с листом отчета
        /// </returns>
        public ExcelPackage CreateNewExcelPackage(string worksheetName);
    }
}
