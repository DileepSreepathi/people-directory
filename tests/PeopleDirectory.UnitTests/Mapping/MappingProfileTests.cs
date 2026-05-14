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
        var person = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Gender = "Male",
            CityId = 1,
            City = new City
            {
                Id = 1,
                Name = "Cape Town",
                CountryId = 1,
                Country = new Country { Id = 1, Name = "South Africa", Code = "ZA" }
            }
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
        var person = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Gender = "Male",
            City = new City
            {
                Id = 1,
                Name = "London",
                Country = new Country { Id = 2, Name = "United Kingdom", Code = "GB" }
            }
        };

        var dto = _mapper.Map<PersonSummaryDto>(person);

        Assert.Equal("John", dto.FirstName);
        Assert.Equal("London", dto.CityName);
        Assert.Equal("United Kingdom", dto.CountryName);
    }

    [Fact]
    public void Person_To_SearchResultDto_MapsFullName()
    {
        var person = new Person
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            City = new City
            {
                Id = 1,
                Name = "Sydney",
                Country = new Country { Id = 3, Name = "Australia", Code = "AU" }
            }
        };

        var dto = _mapper.Map<SearchResultDto>(person);

        Assert.Equal("Jane Smith", dto.FullName);
        Assert.Equal("Sydney", dto.CityName);
        Assert.Equal("Australia", dto.CountryName);
    }

    [Fact]
    public void PersonCreateDto_To_Person_MapsCorrectly()
    {
        var createDto = new PersonCreateDto
        {
            FirstName = "New",
            LastName = "Person",
            Email = "new@example.com",
            CityId = 5,
            Gender = "Female",
            MobileNumber = "+1234567890"
        };

        var person = _mapper.Map<Person>(createDto);

        Assert.Equal("New", person.FirstName);
        Assert.Equal("Person", person.LastName);
        Assert.Equal("new@example.com", person.Email);
        Assert.Equal(5, person.CityId);
        Assert.Equal("Female", person.Gender);
    }

    [Fact]
    public void PersonUpdateDto_To_Person_MapsCorrectly()
    {
        var existing = new Person
        {
            Id = 1,
            FirstName = "Old",
            LastName = "Name",
            Email = "old@example.com",
            CityId = 1
        };
        var updateDto = new PersonUpdateDto
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = "updated@example.com",
            CityId = 2
        };

        _mapper.Map(updateDto, existing);

        Assert.Equal("Updated", existing.FirstName);
        Assert.Equal("updated@example.com", existing.Email);
        Assert.Equal(2, existing.CityId);
        Assert.Equal(1, existing.Id); // ID should not change
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
