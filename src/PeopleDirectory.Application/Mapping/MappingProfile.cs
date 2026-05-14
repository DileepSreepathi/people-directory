using AutoMapper;
using PeopleDirectory.Application.DTOs;
using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Person, PersonSummaryDto>()
            .ForMember(d => d.CityName, opt => opt.MapFrom(s => s.City.Name))
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.City.Country.Name));

        CreateMap<Person, PersonDetailDto>()
            .ForMember(d => d.CityName, opt => opt.MapFrom(s => s.City.Name))
            .ForMember(d => d.CountryId, opt => opt.MapFrom(s => s.City.CountryId))
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.City.Country.Name));

        CreateMap<Person, SearchResultDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
            .ForMember(d => d.CityName, opt => opt.MapFrom(s => s.City.Name))
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.City.Country.Name));

        CreateMap<PersonCreateDto, Person>();
        CreateMap<PersonUpdateDto, Person>();

        CreateMap<Country, CountryDto>();
        CreateMap<City, CityDto>();
    }
}
