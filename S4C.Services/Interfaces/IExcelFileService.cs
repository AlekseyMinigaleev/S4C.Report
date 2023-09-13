using OfficeOpenXml;

namespace C4S.Services.Interfaces
{
    /// <summary>
    /// Сервис excel файлов
    /// </summary>
    public interface IExcelFileService
    {
        /// <summary>
        /// Создает новый лист в <paramref name="package"/> с именем <paramref name="fileName"/>
        /// </summary>
        /// <param name="package"><see cref="ExcelPackage"/>, в котором будет создан новый лист</param>
        /// <param name="fileName">имя с которым будет создан новый лист</param>
        ///<exception cref="InvalidOperationException"/>
        public ExcelWorksheet AddWorksheet(
            ExcelPackage package,
            string fileName);

        /// <summary>
        /// Создает новый excel файл с именем <paramref name="fileName"/>
        /// </summary>
        /// <returns>
        /// <see langword="byte"/>[], представляющий содержимое excel файла
        /// </returns>
        public Task<byte[]> CreateNewFileAsync(
            string fileName,
            CancellationToken cancellationToken);
    }
}
