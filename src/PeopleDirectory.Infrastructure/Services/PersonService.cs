using System.Text.Json;
using AutoMapper;
using PeopleDirectory.Application.DTOs;
using PeopleDirectory.Application.Interfaces;
using PeopleDirectory.Domain.Entities;
using PeopleDirectory.Domain.Interfaces;

namespace PeopleDirectory.Infrastructure.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepo;
    private readonly IAuditLogRepository _auditRepo;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public PersonService(
        IPersonRepository personRepo,
        IAuditLogRepository auditRepo,
        IEmailService emailService,
        IMapper mapper)
    {
        _personRepo = personRepo;
        _auditRepo = auditRepo;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SearchResultDto>> SearchAsync(string query)
    {
        var people = await _personRepo.SearchByNameAsync(query);
        return _mapper.Map<IEnumerable<SearchResultDto>>(people);
    }

    public async Task<PagedResultDto<PersonSummaryDto>> GetFilteredAsync(
        string? query, int? countryId, int? cityId, string? gender, int page, int pageSize, string? sortBy = null)
    {
        var (items, totalCount) = await _personRepo.GetFilteredAsync(query, countryId, cityId, gender, page, pageSize, sortBy);
        return new PagedResultDto<PersonSummaryDto>
        {
            Items = _mapper.Map<IEnumerable<PersonSummaryDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PersonDetailDto?> GetByIdAsync(int id)
    {
        var person = await _personRepo.GetByIdWithDetailsAsync(id);
        return person == null ? null : _mapper.Map<PersonDetailDto>(person);
    }

    public async Task<PersonDetailDto> CreateAsync(PersonCreateDto dto, string? profilePictureUrl, string performedBy)
    {
        var person = _mapper.Map<Person>(dto);
        person.ProfilePictureUrl = profilePictureUrl;
        person.CreatedAt = DateTime.UtcNow;
        person.UpdatedAt = DateTime.UtcNow;

        await _personRepo.AddAsync(person);
        await _personRepo.SaveChangesAsync();

        // Reload with navigation properties
        var created = await _personRepo.GetByIdWithDetailsAsync(person.Id);

        // Audit log
        var audit = new AuditLog
        {
            PersonId = person.Id,
            Action = "Created",
            ChangesJson = JsonSerializer.Serialize(dto),
            PerformedBy = performedBy,
            PerformedAt = DateTime.UtcNow
        };
        await _auditRepo.AddAsync(audit);
        await _auditRepo.SaveChangesAsync();

        // Email notification (fire-and-forget)
        _ = Task.Run(async () =>
        {
            try
            {
                var changes = new Dictionary<string, (string? OldValue, string? NewValue)>
                {
                    ["FirstName"] = (null, dto.FirstName),
                    ["LastName"] = (null, dto.LastName),
                    ["Email"] = (null, dto.Email),
                    ["CityId"] = (null, dto.CityId.ToString()),
                    ["Gender"] = (null, dto.Gender)
                };
                await _emailService.SendChangeNotificationAsync("Created", $"{dto.FirstName} {dto.LastName}", changes);
            }
            catch { /* logged inside email service */ }
        });

        return _mapper.Map<PersonDetailDto>(created);
    }

    public async Task<PersonDetailDto> UpdateAsync(int id, PersonUpdateDto dto, string? profilePictureUrl, string performedBy)
    {
        var person = await _personRepo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Person with ID {id} not found.");

        // Track changes for audit and email
        var changes = new Dictionary<string, (string? OldValue, string? NewValue)>();
        if (person.FirstName != dto.FirstName) changes["FirstName"] = (person.FirstName, dto.FirstName);
        if (person.LastName != dto.LastName) changes["LastName"] = (person.LastName, dto.LastName);
        if (person.Email != dto.Email) changes["Email"] = (person.Email, dto.Email);
        if (person.MobileNumber != dto.MobileNumber) changes["MobileNumber"] = (person.MobileNumber, dto.MobileNumber);
        if (person.Gender != dto.Gender) changes["Gender"] = (person.Gender, dto.Gender);
        if (person.CityId != dto.CityId) changes["CityId"] = (person.CityId.ToString(), dto.CityId.ToString());
        if (person.AddressLine != dto.AddressLine) changes["AddressLine"] = (person.AddressLine, dto.AddressLine);
        if (person.DateOfBirth != dto.DateOfBirth) changes["DateOfBirth"] = (person.DateOfBirth?.ToString(), dto.DateOfBirth?.ToString());
        if (profilePictureUrl != null && person.ProfilePictureUrl != profilePictureUrl)
            changes["ProfilePictureUrl"] = (person.ProfilePictureUrl, profilePictureUrl);

        // Apply changes
        _mapper.Map(dto, person);
        if (profilePictureUrl != null) person.ProfilePictureUrl = profilePictureUrl;
        person.UpdatedAt = DateTime.UtcNow;

        _personRepo.Update(person);
        await _personRepo.SaveChangesAsync();

        // Audit log
        var audit = new AuditLog
        {
            PersonId = person.Id,
            Action = "Updated",
            ChangesJson = JsonSerializer.Serialize(changes),
            PerformedBy = performedBy,
            PerformedAt = DateTime.UtcNow
        };
        await _auditRepo.AddAsync(audit);
        await _auditRepo.SaveChangesAsync();

        // Email notification (fire-and-forget)
        if (changes.Count > 0)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendChangeNotificationAsync("Updated", $"{dto.FirstName} {dto.LastName}", changes);
                }
                catch { /* logged inside email service */ }
            });
        }

        var updated = await _personRepo.GetByIdWithDetailsAsync(id);
        return _mapper.Map<PersonDetailDto>(updated);
    }

    public async Task DeleteAsync(int id, string performedBy)
    {
        var person = await _personRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Person with ID {id} not found.");

        person.IsActive = false;
        person.UpdatedAt = DateTime.UtcNow;
        _personRepo.Update(person);
        await _personRepo.SaveChangesAsync();

        var audit = new AuditLog
        {
            PersonId = person.Id,
            Action = "Deleted",
            PerformedBy = performedBy,
            PerformedAt = DateTime.UtcNow
        };
        await _auditRepo.AddAsync(audit);
        await _auditRepo.SaveChangesAsync();
    }
}
