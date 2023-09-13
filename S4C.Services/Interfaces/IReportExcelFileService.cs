namespace C4S.Services.Interfaces
{
    /// <summary>
    /// Сервис для создания excel файла с отчетами по играм
    /// </summary>
    public interface IReportExcelFileService
    {
        /// <summary>
        /// Создает новый excel файл с отчетами по играм
        /// </summary>
        public Task<byte[]> GetReportAsByteArray(
            CancellationToken cancellationToken);
    }
}
