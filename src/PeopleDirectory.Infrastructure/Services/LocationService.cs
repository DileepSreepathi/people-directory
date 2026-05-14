using AutoMapper;
using PeopleDirectory.Application.DTOs;
using PeopleDirectory.Application.Interfaces;
using PeopleDirectory.Domain.Interfaces;

namespace PeopleDirectory.Infrastructure.Services;

public class LocationService : ILocationService
{
    private readonly ICountryRepository _countryRepo;
    private readonly ICityRepository _cityRepo;
    private readonly IMapper _mapper;

    public LocationService(ICountryRepository countryRepo, ICityRepository cityRepo, IMapper mapper)
    {
        _countryRepo = countryRepo;
        _cityRepo = cityRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
    {
        var countries = await _countryRepo.GetAllAsync();
        return _mapper.Map<IEnumerable<CountryDto>>(countries);
    }

    public async Task<IEnumerable<CityDto>> GetCitiesByCountryAsync(int countryId)
    {
        var cities = await _cityRepo.GetByCountryIdAsync(countryId);
        return _mapper.Map<IEnumerable<CityDto>>(cities);
    }
}
