using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.Domain.Interfaces;

public interface ICountryRepository : IRepository<Country>
{
    Task<IEnumerable<Country>> GetAllWithCitiesAsync();
}
