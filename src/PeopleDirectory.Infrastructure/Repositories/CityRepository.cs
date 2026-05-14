using Microsoft.EntityFrameworkCore;
using PeopleDirectory.Domain.Entities;
using PeopleDirectory.Domain.Interfaces;
using PeopleDirectory.Infrastructure.Data;

namespace PeopleDirectory.Infrastructure.Repositories;

public class CityRepository : Repository<City>, ICityRepository
{
    public CityRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<City>> GetByCountryIdAsync(int countryId)
    {
        return await DbSet
            .Where(c => c.CountryId == countryId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
