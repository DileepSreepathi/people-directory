namespace PeopleDirectory.Application.DTOs;

public class PersonSummaryDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CityName { get; set; }
    public string? CountryName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Gender { get; set; }
}
