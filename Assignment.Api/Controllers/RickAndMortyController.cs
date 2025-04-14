using Assignment.LoaderConsole;
using Assignment.Contracts.Input;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Api.Controllers;

[ApiController]
public class RickAndMortyController(
    ILogger<RickAndMortyController> logger, 
    IRickAndMortyService rickAndMortyService, 
    StatisticsContext statisticsContext
) : ControllerBase
{
    [HttpPost]
    [Route("api/1.0/characters")]
    public async Task<IActionResult> AddCharacter([FromBody] Character newCharacter)
    {
        var character = await rickAndMortyService.AddCharacter(newCharacter);

        return CreatedAtAction(
            nameof(GetCharacter), 
            routeValues: new { id = character.Id }, 
            value: character
        );
    }

    [HttpGet]
    [Route("api/1.0/characters/{id}", Name = nameof(GetCharacter))]
    public async Task<IActionResult> GetCharacter([FromRoute] int id)
    {
        var character = await rickAndMortyService.GetCharacter(id);


        return character is { } c ? Ok(c) : NotFound();
    }

    [HttpGet()]
    [Route("api/1.0/characters")]
    public async Task<IActionResult> FindCharacter([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string? planet = null)
    {
        var results = await rickAndMortyService.FindCharacters(planet, skip, take);

        return Ok(results);
    }
}
