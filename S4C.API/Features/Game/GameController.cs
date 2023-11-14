﻿using C4S.Helpers.ApiHeplers.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using С4S.API.Features.Game.Actions;

namespace С4S.API.Features.Game
{
    public class GameController : BaseApiController
    {
        public GameController(IMediator mediator)
            : base(mediator)
        { }

        /// <summary>
        /// Устанавливает PageId для игры. ОТСТУТСТВУЕТ ВАЛИДАЦИЯ НА УСТАНОВКУ PageId ИГРЕ, КОТОРОЙ ЭТОТ PageId НЕ ПРИНАДЛЕЖИТ.
        /// </summary>
        [HttpPut("SetPageId")]
        public async Task<ActionResult> SetPageIdsAsync(
            [FromBody] SetPageIds.Command request,
            CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Возвращает список всех игр
        /// </summary>
        [HttpGet("GetGames")]
        public async Task<ActionResult> GetGamesAsync(CancellationToken cancellationToken)
        {
            var request = new GetGames.Query();

            var result = await Mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}