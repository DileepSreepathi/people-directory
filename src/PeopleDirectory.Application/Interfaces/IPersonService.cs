using PeopleDirectory.Application.DTOs;

namespace PeopleDirectory.Application.Interfaces;

public interface IPersonService
{
    Task<IEnumerable<SearchResultDto>> SearchAsync(string query);
    Task<PagedResultDto<PersonSummaryDto>> GetFilteredAsync(
        string? query, int? countryId, int? cityId, string? gender, int page, int pageSize, string? sortBy = null);
    Task<PersonDetailDto?> GetByIdAsync(int id);
    Task<PersonDetailDto> CreateAsync(PersonCreateDto dto, string? profilePictureUrl, string performedBy);
    Task<PersonDetailDto> UpdateAsync(int id, PersonUpdateDto dto, string? profilePictureUrl, string performedBy);
    Task DeleteAsync(int id, string performedBy);
}
