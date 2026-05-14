namespace PeopleDirectory.Application.DTOs;

public class PersonDetailDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string? Gender { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int CityId { get; set; }
    public string? CityName { get; set; }
    public int CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? AddressLine { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
