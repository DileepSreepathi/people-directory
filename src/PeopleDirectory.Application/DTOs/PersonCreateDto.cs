namespace PeopleDirectory.Application.DTOs;

public class PersonCreateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int CityId { get; set; }
    public string? AddressLine { get; set; }
}
