using Microsoft.AspNetCore.Mvc;
using PeopleDirectory.Application.Interfaces;

namespace PeopleDirectory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var countries = await _locationService.GetAllCountriesAsync();
        return Ok(countries);
    }

    [HttpGet("countries/{countryId:int}/cities")]
    public async Task<IActionResult> GetCitiesByCountry(int countryId)
    {
        var cities = await _locationService.GetCitiesByCountryAsync(countryId);
        return Ok(cities);
    }
}
