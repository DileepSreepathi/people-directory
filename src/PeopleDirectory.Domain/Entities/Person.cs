namespace PeopleDirectory.Domain.Entities;

public class Person
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
    public string? AddressLine { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public City City { get; set; } = null!;
}
