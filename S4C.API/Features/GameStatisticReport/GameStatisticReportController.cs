using C4S.DB.DTO;
using C4S.Helpers.ApiHeplers.Controllers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using С4S.API.Features.GameStatisticReport.Actions;

namespace С4S.API.Features.GameStatisticReport
{
    public class GameStatisticReportController : BaseApiController
    {
        public GameStatisticReportController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Возвращает готовый к скачиванию файл отчета игровой статистики.
        /// </summary>
        /// <remarks>
        /// dd.MM.yyyy
        /// </remarks>
        [HttpGet("DownloadReport")]
        public async Task<ActionResult> GetGameStatisticReportAsync(
            [FromQuery]string startDate,
            [FromQuery]string finishDate,
            [FromServices]IValidator<DateTimeRange> dateTimeRangeValidator,
            [FromServices] IValidator<GetReportFileWithGameStatistics.Query> queryValidator,
            CancellationToken cancellationToken = default)
        {
            /*TODO: исправать после добавления фронта*/
            var dateTimeFormat = "dd.MM.yyyy";
            var dateTimeRange = new DateTimeRange(
                startDate: DateTime.ParseExact(startDate,dateTimeFormat , CultureInfo.InvariantCulture),
                finishDate: DateTime.ParseExact(finishDate, dateTimeFormat, CultureInfo.InvariantCulture));
            await ValidateAndChangeModelStateAsync(dateTimeRangeValidator, dateTimeRange, cancellationToken);

            var query = new GetReportFileWithGameStatistics.Query { DateRange = dateTimeRange};
            await ValidateAndChangeModelStateAsync(queryValidator, query, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var file = await Mediator.Send(query, cancellationToken);

            return File(file.Bytes,file.MimeType,file.Name);
        }
    }
}
