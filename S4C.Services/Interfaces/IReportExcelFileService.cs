namespace C4S.Services.Interfaces
{
    /// <summary>
    /// Сервис для создания excel файла с отчетами по играм
    /// </summary>
    public interface IReportExcelFileService
    {
        /// <summary>
        /// Создает новый excel файл со всеми отчетами по играм
        /// </summary>
        /// <returns>
        /// <see langword="byte"/>[], представляющий содержимое excel файла со всеми отчетами по играм
        /// </returns>
        public Task<byte[]> GetReportFile(
            CancellationToken cancellationToken);
    }
}
