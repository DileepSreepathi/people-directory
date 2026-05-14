using AutoMapper;
using Moq;
using PeopleDirectory.Application.DTOs;
using PeopleDirectory.Application.Interfaces;
using PeopleDirectory.Application.Mapping;
using PeopleDirectory.Domain.Entities;
using PeopleDirectory.Domain.Interfaces;
using PeopleDirectory.Infrastructure.Services;

namespace PeopleDirectory.UnitTests.Services;

public class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _personRepoMock;
    private readonly Mock<IAuditLogRepository> _auditRepoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly IMapper _mapper;
    private readonly PersonService _sut;

    public PersonServiceTests()
    {
        _personRepoMock = new Mock<IPersonRepository>();
        _auditRepoMock = new Mock<IAuditLogRepository>();
        _emailServiceMock = new Mock<IEmailService>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new PersonService(_personRepoMock.Object, _auditRepoMock.Object, _emailServiceMock.Object, _mapper);
    }

    private static Person CreateSamplePerson(int id = 1) => new()
    {
        Id = id,
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        MobileNumber = "+1234567890",
        Gender = "Male",
        CityId = 1,
        AddressLine = "123 Main St",
        DateOfBirth = new DateOnly(1990, 1, 15),
        IsActive = true,
        City = new City
        {
            Id = 1,
            Name = "Cape Town",
            CountryId = 1,
            Country = new Country { Id = 1, Name = "South Africa", Code = "ZA" }
        }
    };

    // ── SearchAsync ──

    [Fact]
    public async Task SearchAsync_ReturnsMatchingResults()
    {
        var people = new List<Person> { CreateSamplePerson() };
        _personRepoMock.Setup(r => r.SearchByNameAsync("John", 10)).ReturnsAsync(people);

        var results = (await _sut.SearchAsync("John")).ToList();

        Assert.Single(results);
        Assert.Equal("John Doe", results[0].FullName);
        Assert.Equal("Cape Town", results[0].CityName);
    }

    [Fact]
    public async Task SearchAsync_EmptyResults_ReturnsEmptyList()
    {
        _personRepoMock.Setup(r => r.SearchByNameAsync("XYZ", 10)).ReturnsAsync(new List<Person>());

        var results = await _sut.SearchAsync("XYZ");

        Assert.Empty(results);
    }

    // ── GetFilteredAsync ──

    [Fact]
    public async Task GetFilteredAsync_ReturnsPaginatedResults()
    {
        var people = new List<Person> { CreateSamplePerson() };
        _personRepoMock.Setup(r => r.GetFilteredAsync(null, null, null, null, 1, 10, null))
            .ReturnsAsync((people.AsEnumerable(), 1));

        var result = await _sut.GetFilteredAsync(null, null, null, null, 1, 10);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Single(result.Items);
    }

    // ── GetByIdAsync ──

    [Fact]
    public async Task GetByIdAsync_ExistingPerson_ReturnsDetail()
    {
        var person = CreateSamplePerson();
        _personRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(person);

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("John", result!.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("Cape Town", result.CityName);
        Assert.Equal("South Africa", result.CountryName);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingPerson_ReturnsNull()
    {
        _personRepoMock.Setup(r => r.GetByIdWithDetailsAsync(999)).ReturnsAsync((Person?)null);

        var result = await _sut.GetByIdAsync(999);

        Assert.Null(result);
    }

    // ── CreateAsync ──

    [Fact]
    public async Task CreateAsync_SavesPersonAndAuditLog()
    {
        var dto = new PersonCreateDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            CityId = 1,
            Gender = "Female"
        };
        var createdPerson = CreateSamplePerson(5);
        createdPerson.FirstName = "Jane";
        createdPerson.LastName = "Smith";

        _personRepoMock.Setup(r => r.AddAsync(It.IsAny<Person>())).Returns(Task.CompletedTask);
        _personRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _personRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(createdPerson);
        _auditRepoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
        _auditRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto, "/uploads/pic.jpg", "admin");

        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
        _personRepoMock.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Once);
        _personRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _auditRepoMock.Verify(r => r.AddAsync(It.Is<AuditLog>(a => a.Action == "Created")), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsProfilePictureUrl()
    {
        var dto = new PersonCreateDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            CityId = 1
        };
        var createdPerson = CreateSamplePerson(5);

        _personRepoMock.Setup(r => r.AddAsync(It.IsAny<Person>())).Returns(Task.CompletedTask);
        _personRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _personRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(createdPerson);
        _auditRepoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
        _auditRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.CreateAsync(dto, "/uploads/test.jpg", "admin");

        _personRepoMock.Verify(r => r.AddAsync(It.Is<Person>(p => p.ProfilePictureUrl == "/uploads/test.jpg")), Times.Once);
    }

    // ── UpdateAsync ──

    [Fact]
    public async Task UpdateAsync_ExistingPerson_TracksChanges()
    {
        var existing = CreateSamplePerson();
        var dto = new PersonUpdateDto
        {
            FirstName = "Johnny",
            LastName = "Doe",
            Email = "john@example.com",
            MobileNumber = "+1234567890",
            Gender = "Male",
            CityId = 1,
            AddressLine = "456 New St",
            DateOfBirth = new DateOnly(1990, 1, 15)
        };

        _personRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(existing);
        _personRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _auditRepoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
        _auditRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.UpdateAsync(1, dto, null, "admin");

        _auditRepoMock.Verify(r => r.AddAsync(It.Is<AuditLog>(a =>
            a.Action == "Updated" && a.PersonId == 1)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingPerson_ThrowsKeyNotFoundException()
    {
        _personRepoMock.Setup(r => r.GetByIdWithDetailsAsync(999)).ReturnsAsync((Person?)null);

        var dto = new PersonUpdateDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            CityId = 1
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.UpdateAsync(999, dto, null, "admin"));
    }

    [Fact]
    public async Task UpdateAsync_NoChanges_DoesNotSendEmail()
    {
        var existing = CreateSamplePerson();
        var dto = new PersonUpdateDto
        {
            FirstName = existing.FirstName,
            LastName = existing.LastName,
            Email = existing.Email,
            MobileNumber = existing.MobileNumber,
            Gender = existing.Gender,
            CityId = existing.CityId,
            AddressLine = existing.AddressLine,
            DateOfBirth = existing.DateOfBirth
        };

        _personRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(existing);
        _personRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _auditRepoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
        _auditRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.UpdateAsync(1, dto, null, "admin");

        // Give fire-and-forget a moment, then verify no email was sent
        await Task.Delay(100);
        _emailServiceMock.Verify(e => e.SendChangeNotificationAsync(
            It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<Dictionary<string, (string?, string?)>>()), Times.Never);
    }

    // ── DeleteAsync ──

    [Fact]
    public async Task DeleteAsync_ExistingPerson_SoftDeletes()
    {
        var person = CreateSamplePerson();
        _personRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(person);
        _personRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _auditRepoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
        _auditRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteAsync(1, "admin");

        Assert.False(person.IsActive);
        _personRepoMock.Verify(r => r.Update(It.Is<Person>(p => !p.IsActive)), Times.Once);
        _auditRepoMock.Verify(r => r.AddAsync(It.Is<AuditLog>(a => a.Action == "Deleted")), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingPerson_ThrowsKeyNotFoundException()
    {
        _personRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Person?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.DeleteAsync(999, "admin"));
    }
}
