using Microsoft.EntityFrameworkCore;
using PeopleDirectory.Domain.Entities;
using PeopleDirectory.Domain.Interfaces;
using PeopleDirectory.Infrastructure.Data;

namespace PeopleDirectory.Infrastructure.Repositories;

public class PersonRepository : Repository<Person>, IPersonRepository
{
    public PersonRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Person>> SearchByNameAsync(string query, int take = 10)
    {
        var lowerQuery = query.ToLower();
        return await DbSet
            .Include(p => p.City)
                .ThenInclude(c => c.Country)
            .Where(p => p.FirstName.ToLower().Contains(lowerQuery) ||
                        p.LastName.ToLower().Contains(lowerQuery))
            .OrderBy(p => p.FirstName)
            .Take(take)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Person> Items, int TotalCount)> GetFilteredAsync(
        string? query, int? countryId, int? cityId, string? gender, int page, int pageSize, string? sortBy = null)
    {
        var queryable = DbSet
            .Include(p => p.City)
                .ThenInclude(c => c.Country)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            queryable = queryable.Where(p =>
                p.FirstName.ToLower().Contains(lowerQuery) ||
                p.LastName.ToLower().Contains(lowerQuery));
        }

        if (countryId.HasValue)
            queryable = queryable.Where(p => p.City.CountryId == countryId.Value);

        if (cityId.HasValue)
            queryable = queryable.Where(p => p.CityId == cityId.Value);

        if (!string.IsNullOrWhiteSpace(gender))
            queryable = queryable.Where(p => p.Gender == gender);

        queryable = (sortBy?.ToLowerInvariant()) switch
        {
            "name_desc"   => queryable.OrderByDescending(p => p.LastName).ThenByDescending(p => p.FirstName),
            "newest"      => queryable.OrderByDescending(p => p.CreatedAt),
            "oldest"      => queryable.OrderBy(p => p.CreatedAt),
            "country"     => queryable.OrderBy(p => p.City.Country.Name).ThenBy(p => p.LastName),
            _              => queryable.OrderBy(p => p.LastName).ThenBy(p => p.FirstName),
        };

        var totalCount = await queryable.CountAsync();
        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Person?> GetByIdWithDetailsAsync(int id)
    {
        return await DbSet
            .Include(p => p.City)
                .ThenInclude(c => c.Country)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
