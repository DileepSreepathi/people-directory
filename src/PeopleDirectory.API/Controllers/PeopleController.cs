using Microsoft.AspNetCore.Mvc;
using PeopleDirectory.Application.Interfaces;

namespace PeopleDirectory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly IPersonService _personService;

    public PeopleController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Predictive type-ahead search — returns top 10 matches by FirstName or LastName.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(Array.Empty<object>());

        var results = await _personService.SearchAsync(query);
        return Ok(results);
    }

    /// <summary>
    /// Full search with filters, sorting, and pagination.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? query,
        [FromQuery] int? countryId,
        [FromQuery] int? cityId,
        [FromQuery] string? gender,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        var result = await _personService.GetFilteredAsync(query, countryId, cityId, gender, page, pageSize, sortBy);
        return Ok(result);
    }

    /// <summary>
    /// Get full person detail by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var person = await _personService.GetByIdAsync(id);
        if (person == null)
            return NotFound(new { message = $"Person with ID {id} not found." });

        return Ok(person);
    }
}
