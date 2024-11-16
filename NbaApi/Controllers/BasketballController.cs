using Microsoft.AspNetCore.Mvc;
using NbaApi.Models;
using NbaApi.Services;

namespace NbaApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketballController : ControllerBase
{
    private readonly IBasketballService _basketballService;

    public BasketballController(IBasketballService basketballService)
    {
        _basketballService = basketballService;
    }

    [HttpGet("teams")]
    public async Task<IActionResult> GetTeamsWithOrderedPlayers()
    {
        var teams = await _basketballService.GetTeamsWithOrderedPlayers();
        return Ok(teams);
    }
}