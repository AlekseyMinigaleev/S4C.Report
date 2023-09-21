﻿using C4S.DB.DTO;
using OfficeOpenXml;

namespace C4S.Services.Interfaces
{
    /// <summary>
    /// Сервис excel файлов
    /// </summary>
    public interface IExcelWorksheetService
    {
        /// <summary>
        /// Создает новый лист с отчетом в <paramref name="package"/>, с данными за период <paramref name="dateTimeRange"/> и именем <paramref name="worksheetName"/>/>
        /// </summary>
        /// <param name="package"><see cref="ExcelPackage"/>, в котором будет создан новый лист</param>
        /// <param name="dateTimeRange"> Период за который будет создан отчет</param>
        /// <param name="worksheetName">имя с которым будет создан новый лист</param>
        ///<exception cref="InvalidOperationException"/>
        public ExcelWorksheet Add(
            ExcelPackage package,
            string worksheetName,
            DateTimeRange dateTimeRange);

        /// <summary>
        /// Создает новый <see cref="ExcelPackage"/>, с данными за период <paramref name="dateTimeRange"/> и именем <paramref name="worksheetName"/>
        /// </summary>
        /// <param name="worksheetName"> имя с которым будет создан новый лист</param>
        /// <param name="dateTimeRange"> Период за который будет создан отчет</param>
        /// <returns>
        /// <see cref="ExcelPackage"/>[], представляющий содержимое excel файла с новым листом
        /// </returns>
        public ExcelPackage AddWithNewPackage(
            string worksheetName,
            DateTimeRange dateTimeRange);
    }
}