using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.Domain.Interfaces;

public interface IPersonRepository : IRepository<Person>
{
    Task<IEnumerable<Person>> SearchByNameAsync(string query, int take = 10);
    Task<(IEnumerable<Person> Items, int TotalCount)> GetFilteredAsync(
        string? query, int? countryId, int? cityId, string? gender, int page, int pageSize, string? sortBy = null);
    Task<Person?> GetByIdWithDetailsAsync(int id);
}
