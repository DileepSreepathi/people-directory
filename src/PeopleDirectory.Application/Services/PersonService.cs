using System.Text.Json;
using AutoMapper;
using PeopleDirectory.Application.DTOs;
using PeopleDirectory.Application.Interfaces;
using PeopleDirectory.Application.Notifications;
using PeopleDirectory.Domain.Entities;
using PeopleDirectory.Domain.Interfaces;

namespace PeopleDirectory.Application.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepo;
    private readonly IAuditLogRepository _auditRepo;
    private readonly IOutboxRepository _outboxRepo;
    private readonly IMapper _mapper;

    public PersonService(
        IPersonRepository personRepo,
        IAuditLogRepository auditRepo,
        IOutboxRepository outboxRepo,
        IMapper mapper)
    {
        _personRepo = personRepo;
        _auditRepo = auditRepo;
        _outboxRepo = outboxRepo;
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
        var person = Person.Create(
            dto.FirstName, dto.LastName, dto.Email, dto.MobileNumber, dto.Gender,
            dto.CityId, dto.AddressLine, dto.DateOfBirth, profilePictureUrl);

        await _personRepo.AddAsync(person);
        await _personRepo.SaveChangesAsync();

        // Reload with navigation properties
        var created = await _personRepo.GetByIdWithDetailsAsync(person.Id);

        var changes = new List<FieldChange>
        {
            new(nameof(dto.FirstName), null, dto.FirstName),
            new(nameof(dto.LastName), null, dto.LastName),
            new(nameof(dto.Email), null, dto.Email),
            new(nameof(dto.CityId), null, dto.CityId.ToString()),
            new(nameof(dto.Gender), null, dto.Gender)
        };

        await WriteAuditAndEnqueueNotificationAsync(
            person.Id, "Created", $"{dto.FirstName} {dto.LastName}", changes, performedBy);

        return _mapper.Map<PersonDetailDto>(created);
    }

    public async Task<PersonDetailDto> UpdateAsync(int id, PersonUpdateDto dto, string? profilePictureUrl, string performedBy)
    {
        var person = await _personRepo.GetByIdWithDetailsAsync(id)
            ?? throw new KeyNotFoundException($"Person with ID {id} not found.");

        // The entity owns change detection and its own invariants.
        var changes = person.ApplyUpdate(
            dto.FirstName, dto.LastName, dto.Email, dto.MobileNumber, dto.Gender,
            dto.CityId, dto.AddressLine, dto.DateOfBirth, profilePictureUrl);

        _personRepo.Update(person);
        await _personRepo.SaveChangesAsync();

        await WriteAuditAndEnqueueNotificationAsync(
            person.Id, "Updated", $"{dto.FirstName} {dto.LastName}", changes, performedBy,
            enqueueNotification: changes.Count > 0);

        var updated = await _personRepo.GetByIdWithDetailsAsync(id);
        return _mapper.Map<PersonDetailDto>(updated);
    }

    public async Task DeleteAsync(int id, string performedBy)
    {
        var person = await _personRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Person with ID {id} not found.");

        person.Deactivate();
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

    /// <summary>
    /// Writes the audit log and (optionally) stages the change-notification in the
    /// outbox, committing both in a single SaveChanges so they share one transaction.
    /// The email itself is dispatched later by the background outbox processor, which
    /// gives at-least-once delivery instead of best-effort fire-and-forget.
    /// </summary>
    private async Task WriteAuditAndEnqueueNotificationAsync(
        int personId,
        string action,
        string personName,
        IReadOnlyList<FieldChange> changes,
        string performedBy,
        bool enqueueNotification = true)
    {
        var audit = new AuditLog
        {
            PersonId = personId,
            Action = action,
            ChangesJson = JsonSerializer.Serialize(changes),
            PerformedBy = performedBy,
            PerformedAt = DateTime.UtcNow
        };
        await _auditRepo.AddAsync(audit);

        if (enqueueNotification)
        {
            var payload = new ChangeNotificationPayload(action, personName, changes.ToList());
            var message = OutboxMessage.Create(
                ChangeNotificationPayload.MessageType,
                JsonSerializer.Serialize(payload));
            await _outboxRepo.AddAsync(message);
        }

        // Single commit: audit + outbox row persist atomically (shared DbContext).
        await _auditRepo.SaveChangesAsync();
    }
}
