using Microsoft.EntityFrameworkCore;
using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.Infrastructure.Data;

public static class SeedData
{
    public static void Seed(ModelBuilder builder)
    {
        // Countries
        var countries = new[]
        {
            new Country { Id = 1, Name = "South Africa", Code = "ZA" },
            new Country { Id = 2, Name = "United States", Code = "US" },
            new Country { Id = 3, Name = "United Kingdom", Code = "GB" },
            new Country { Id = 4, Name = "Australia", Code = "AU" },
            new Country { Id = 5, Name = "Canada", Code = "CA" },
            new Country { Id = 6, Name = "Germany", Code = "DE" },
            new Country { Id = 7, Name = "India", Code = "IN" },
            new Country { Id = 8, Name = "Brazil", Code = "BR" },
            new Country { Id = 9, Name = "Japan", Code = "JP" },
            new Country { Id = 10, Name = "France", Code = "FR" }
        };
        builder.Entity<Country>().HasData(countries);

        // Cities
        var cities = new[]
        {
            // South Africa
            new City { Id = 1, Name = "Cape Town", CountryId = 1 },
            new City { Id = 2, Name = "Johannesburg", CountryId = 1 },
            new City { Id = 3, Name = "Durban", CountryId = 1 },
            new City { Id = 4, Name = "Pretoria", CountryId = 1 },
            // United States
            new City { Id = 5, Name = "New York", CountryId = 2 },
            new City { Id = 6, Name = "Los Angeles", CountryId = 2 },
            new City { Id = 7, Name = "Chicago", CountryId = 2 },
            new City { Id = 8, Name = "Houston", CountryId = 2 },
            // United Kingdom
            new City { Id = 9, Name = "London", CountryId = 3 },
            new City { Id = 10, Name = "Manchester", CountryId = 3 },
            new City { Id = 11, Name = "Birmingham", CountryId = 3 },
            // Australia
            new City { Id = 12, Name = "Sydney", CountryId = 4 },
            new City { Id = 13, Name = "Melbourne", CountryId = 4 },
            new City { Id = 14, Name = "Brisbane", CountryId = 4 },
            // Canada
            new City { Id = 15, Name = "Toronto", CountryId = 5 },
            new City { Id = 16, Name = "Vancouver", CountryId = 5 },
            new City { Id = 17, Name = "Montreal", CountryId = 5 },
            // Germany
            new City { Id = 18, Name = "Berlin", CountryId = 6 },
            new City { Id = 19, Name = "Munich", CountryId = 6 },
            // India
            new City { Id = 20, Name = "Mumbai", CountryId = 7 },
            new City { Id = 21, Name = "Delhi", CountryId = 7 },
            new City { Id = 22, Name = "Bangalore", CountryId = 7 },
            // Brazil
            new City { Id = 23, Name = "São Paulo", CountryId = 8 },
            new City { Id = 24, Name = "Rio de Janeiro", CountryId = 8 },
            // Japan
            new City { Id = 25, Name = "Tokyo", CountryId = 9 },
            new City { Id = 26, Name = "Osaka", CountryId = 9 },
            // France
            new City { Id = 27, Name = "Paris", CountryId = 10 },
            new City { Id = 28, Name = "Lyon", CountryId = 10 }
        };
        builder.Entity<City>().HasData(cities);

        // Sample People
        // Anonymous objects are used here on purpose: EF Core HasData maps them
        // by property name and does not require public setters, so the Person
        // entity can keep its business properties encapsulated (private set).
        var people = new object[]
        {
            new { Id = 1, FirstName = "Mark", LastName = "Johnson", Email = "mark.johnson@example.com", MobileNumber = "+27821234567", Gender = "Male", CityId = 1, AddressLine = "123 Long Street", DateOfBirth = new DateOnly(1990, 5, 15), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 2, FirstName = "Sarah", LastName = "Williams", Email = "sarah.williams@example.com", MobileNumber = "+27829876543", Gender = "Female", CityId = 2, AddressLine = "456 Main Road", DateOfBirth = new DateOnly(1985, 8, 22), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 3, FirstName = "James", LastName = "Smith", Email = "james.smith@example.com", MobileNumber = "+12125551234", Gender = "Male", CityId = 5, AddressLine = "789 Broadway", DateOfBirth = new DateOnly(1992, 3, 10), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 4, FirstName = "Emily", LastName = "Brown", Email = "emily.brown@example.com", MobileNumber = "+447911123456", Gender = "Female", CityId = 9, AddressLine = "10 Baker Street", DateOfBirth = new DateOnly(1988, 11, 30), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 5, FirstName = "Michael", LastName = "Davis", Email = "michael.davis@example.com", MobileNumber = "+61412345678", Gender = "Male", CityId = 12, AddressLine = "55 George Street", DateOfBirth = new DateOnly(1995, 7, 4), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 6, FirstName = "Jessica", LastName = "Wilson", Email = "jessica.wilson@example.com", MobileNumber = "+14161234567", Gender = "Female", CityId = 15, AddressLine = "100 Queen Street", DateOfBirth = new DateOnly(1993, 2, 14), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 7, FirstName = "David", LastName = "Taylor", Email = "david.taylor@example.com", MobileNumber = "+493012345678", Gender = "Male", CityId = 18, AddressLine = "22 Unter den Linden", DateOfBirth = new DateOnly(1987, 9, 25), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 8, FirstName = "Priya", LastName = "Sharma", Email = "priya.sharma@example.com", MobileNumber = "+919876543210", Gender = "Female", CityId = 20, AddressLine = "15 Marine Drive", DateOfBirth = new DateOnly(1991, 12, 3), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 9, FirstName = "Lucas", LastName = "Silva", Email = "lucas.silva@example.com", MobileNumber = "+5511987654321", Gender = "Male", CityId = 23, AddressLine = "Av. Paulista 1000", DateOfBirth = new DateOnly(1994, 6, 18), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 10, FirstName = "Yuki", LastName = "Tanaka", Email = "yuki.tanaka@example.com", MobileNumber = "+81312345678", Gender = "Female", CityId = 25, AddressLine = "3-1 Shibuya", DateOfBirth = new DateOnly(1996, 4, 7), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 11, FirstName = "Sophie", LastName = "Dupont", Email = "sophie.dupont@example.com", MobileNumber = "+33612345678", Gender = "Female", CityId = 27, AddressLine = "8 Rue de Rivoli", DateOfBirth = new DateOnly(1989, 1, 20), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 12, FirstName = "Mark", LastName = "van der Berg", Email = "mark.vdberg@example.com", MobileNumber = "+27831112233", Gender = "Male", CityId = 1, AddressLine = "45 Kloof Street", DateOfBirth = new DateOnly(1986, 10, 12), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 13, FirstName = "Olivia", LastName = "Martinez", Email = "olivia.martinez@example.com", MobileNumber = "+12135557890", Gender = "Female", CityId = 6, AddressLine = "321 Sunset Blvd", DateOfBirth = new DateOnly(1997, 3, 28), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 14, FirstName = "Raj", LastName = "Patel", Email = "raj.patel@example.com", MobileNumber = "+919123456789", Gender = "Male", CityId = 22, AddressLine = "100 MG Road", DateOfBirth = new DateOnly(1990, 8, 5), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 15, FirstName = "Emma", LastName = "Thompson", Email = "emma.thompson@example.com", MobileNumber = "+447922987654", Gender = "Female", CityId = 10, AddressLine = "5 Deansgate", DateOfBirth = new DateOnly(1992, 5, 30), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 16, FirstName = "Liam", LastName = "O'Brien", Email = "liam.obrien@example.com", MobileNumber = "+61423456789", Gender = "Male", CityId = 13, AddressLine = "88 Collins Street", DateOfBirth = new DateOnly(1988, 7, 14), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 17, FirstName = "Ana", LastName = "Costa", Email = "ana.costa@example.com", MobileNumber = "+5521912345678", Gender = "Female", CityId = 24, AddressLine = "Rua Copacabana 50", DateOfBirth = new DateOnly(1995, 11, 2), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 18, FirstName = "Thomas", LastName = "Mueller", Email = "thomas.mueller@example.com", MobileNumber = "+498912345678", Gender = "Male", CityId = 19, AddressLine = "15 Marienplatz", DateOfBirth = new DateOnly(1984, 4, 19), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 19, FirstName = "Chloe", LastName = "Nguyen", Email = "chloe.nguyen@example.com", MobileNumber = "+16041234567", Gender = "Female", CityId = 16, AddressLine = "200 Robson Street", DateOfBirth = new DateOnly(1993, 9, 8), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 20, FirstName = "Sipho", LastName = "Nkosi", Email = "sipho.nkosi@example.com", MobileNumber = "+27841234567", Gender = "Male", CityId = 3, AddressLine = "78 Florida Road", DateOfBirth = new DateOnly(1991, 2, 27), CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        builder.Entity<Person>().HasData(people);
    }
}
