using AutoMapper;
using PeopleDirectory.Application.DTOs;
using PeopleDirectory.Application.Mapping;
using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.UnitTests.Mapping;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void MappingProfile_Configuration_IsValid()
    {
        // Verify the profile can be created without throwing
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        Assert.NotNull(mapper);
    }

    [Fact]
    public void Person_To_PersonDetailDto_MapsCorrectly()
    {
        var person = Person.Create("John", "Doe", "john@example.com", null, "Male", 1, null, null, null);
        person.Id = 1;
        person.City = new City
        {
            Id = 1,
            Name = "Cape Town",
            CountryId = 1,
            Country = new Country { Id = 1, Name = "South Africa", Code = "ZA" }
        };

        var dto = _mapper.Map<PersonDetailDto>(person);

        Assert.Equal(1, dto.Id);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Cape Town", dto.CityName);
        Assert.Equal(1, dto.CountryId);
        Assert.Equal("South Africa", dto.CountryName);
    }

    [Fact]
    public void Person_To_PersonSummaryDto_MapsCorrectly()
    {
        var person = Person.Create("John", "Doe", "john@example.com", null, "Male", 1, null, null, null);
        person.Id = 1;
        person.City = new City
        {
            Id = 1,
            Name = "London",
            Country = new Country { Id = 2, Name = "United Kingdom", Code = "GB" }
        };

        var dto = _mapper.Map<PersonSummaryDto>(person);

        Assert.Equal("John", dto.FirstName);
        Assert.Equal("London", dto.CityName);
        Assert.Equal("United Kingdom", dto.CountryName);
    }

    [Fact]
    public void Person_To_SearchResultDto_MapsFullName()
    {
        var person = Person.Create("Jane", "Smith", "jane@example.com", null, null, 1, null, null, null);
        person.Id = 1;
        person.City = new City
        {
            Id = 1,
            Name = "Sydney",
            Country = new Country { Id = 3, Name = "Australia", Code = "AU" }
        };

        var dto = _mapper.Map<SearchResultDto>(person);

        Assert.Equal("Jane Smith", dto.FullName);
        Assert.Equal("Sydney", dto.CityName);
        Assert.Equal("Australia", dto.CountryName);
    }

    [Fact]
    public void Country_To_CountryDto_MapsCorrectly()
    {
        var country = new Country { Id = 1, Name = "Brazil", Code = "BR" };
        var dto = _mapper.Map<CountryDto>(country);

        Assert.Equal(1, dto.Id);
        Assert.Equal("Brazil", dto.Name);
        Assert.Equal("BR", dto.Code);
    }

    [Fact]
    public void City_To_CityDto_MapsCorrectly()
    {
        var city = new City { Id = 5, Name = "Rio de Janeiro", CountryId = 3 };
        var dto = _mapper.Map<CityDto>(city);

        Assert.Equal(5, dto.Id);
        Assert.Equal("Rio de Janeiro", dto.Name);
        Assert.Equal(3, dto.CountryId);
    }
}
