using C4S.Helpers.ApiHeplers.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("DownloadReport")]
        public async Task<ActionResult> GetGameStatisticReportAsync(
            CancellationToken cancellation = default)
        {
            var query = new GetGameStatisticReportFile.Query();

            var file = await Mediator.Send(query, cancellation);

            return File(file.Bytes,file.MimeType,file.Name);
        }
    }
}
