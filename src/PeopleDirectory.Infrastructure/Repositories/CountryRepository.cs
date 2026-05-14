using Microsoft.EntityFrameworkCore;
using PeopleDirectory.Domain.Entities;
using PeopleDirectory.Domain.Interfaces;
using PeopleDirectory.Infrastructure.Data;

namespace PeopleDirectory.Infrastructure.Repositories;

public class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Country>> GetAllWithCitiesAsync()
    {
        return await DbSet.Include(c => c.Cities).OrderBy(c => c.Name).ToListAsync();
    }
}
