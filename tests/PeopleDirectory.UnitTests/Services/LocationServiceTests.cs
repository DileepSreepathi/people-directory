using AutoMapper;
using Moq;
using PeopleDirectory.Application.DTOs;
using PeopleDirectory.Application.Mapping;
using PeopleDirectory.Domain.Entities;
using PeopleDirectory.Domain.Interfaces;
using PeopleDirectory.Infrastructure.Services;

namespace PeopleDirectory.UnitTests.Services;

public class LocationServiceTests
{
    private readonly Mock<ICountryRepository> _countryRepoMock;
    private readonly Mock<ICityRepository> _cityRepoMock;
    private readonly IMapper _mapper;
    private readonly LocationService _sut;

    public LocationServiceTests()
    {
        _countryRepoMock = new Mock<ICountryRepository>();
        _cityRepoMock = new Mock<ICityRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new LocationService(_countryRepoMock.Object, _cityRepoMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllCountriesAsync_ReturnsAllCountries()
    {
        var countries = new List<Country>
        {
            new() { Id = 1, Name = "South Africa", Code = "ZA" },
            new() { Id = 2, Name = "United Kingdom", Code = "GB" },
            new() { Id = 3, Name = "United States", Code = "US" }
        };
        _countryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(countries);

        var result = (await _sut.GetAllCountriesAsync()).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("South Africa", result[0].Name);
        Assert.Equal("ZA", result[0].Code);
    }

    [Fact]
    public async Task GetAllCountriesAsync_NoCountries_ReturnsEmpty()
    {
        _countryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Country>());

        var result = await _sut.GetAllCountriesAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCitiesByCountryAsync_ReturnsCitiesForCountry()
    {
        var cities = new List<City>
        {
            new() { Id = 1, Name = "Cape Town", CountryId = 1 },
            new() { Id = 2, Name = "Johannesburg", CountryId = 1 },
            new() { Id = 3, Name = "Durban", CountryId = 1 }
        };
        _cityRepoMock.Setup(r => r.GetByCountryIdAsync(1)).ReturnsAsync(cities);

        var result = (await _sut.GetCitiesByCountryAsync(1)).ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, c => Assert.Equal(1, c.CountryId));
        Assert.Equal("Cape Town", result[0].Name);
    }

    [Fact]
    public async Task GetCitiesByCountryAsync_NoMatch_ReturnsEmpty()
    {
        _cityRepoMock.Setup(r => r.GetByCountryIdAsync(999)).ReturnsAsync(new List<City>());

        var result = await _sut.GetCitiesByCountryAsync(999);

        Assert.Empty(result);
    }
}
