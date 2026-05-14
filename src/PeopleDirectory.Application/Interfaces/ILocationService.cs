using PeopleDirectory.Application.DTOs;

namespace PeopleDirectory.Application.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
    Task<IEnumerable<CityDto>> GetCitiesByCountryAsync(int countryId);
}
