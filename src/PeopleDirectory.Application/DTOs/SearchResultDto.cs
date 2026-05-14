namespace PeopleDirectory.Application.DTOs;

public class SearchResultDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? CityName { get; set; }
    public string? CountryName { get; set; }
}
