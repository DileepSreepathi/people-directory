using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.Domain.Interfaces;

public interface ICityRepository : IRepository<City>
{
    Task<IEnumerable<City>> GetByCountryIdAsync(int countryId);
}
