using C4S.ApiHelpers.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace С4S.API.Features.Parsers
{
    public class ParserController :BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<int>> Parser()
        {
            await Mediator.Send(new GameStatisticParser.Query());

            return Ok(1);
        }
    }
}
