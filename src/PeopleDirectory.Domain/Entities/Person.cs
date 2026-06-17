namespace PeopleDirectory.Domain.Entities;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? MobileNumber { get; private set; }
    public string? Gender { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public int CityId { get; private set; }
    public string? AddressLine { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsActive { get; private set; } = true;

    public City City { get; set; } = null!;

    /// <summary>
    /// Factory for creating a new person. Keeps construction and the
    /// associated invariants (timestamps, active flag) inside the domain
    /// rather than letting callers set them ad hoc.
    /// </summary>
    public static Person Create(
        string firstName,
        string lastName,
        string email,
        string? mobileNumber,
        string? gender,
        int cityId,
        string? addressLine,
        DateOnly? dateOfBirth,
        string? profilePictureUrl)
    {
        var now = DateTime.UtcNow;
        return new Person
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            MobileNumber = mobileNumber,
            Gender = gender,
            CityId = cityId,
            AddressLine = addressLine,
            DateOfBirth = dateOfBirth,
            ProfilePictureUrl = profilePictureUrl,
            CreatedAt = now,
            UpdatedAt = now,
            IsActive = true
        };
    }

    /// <summary>
    /// Applies an update to this person, returning the set of fields that
    /// actually changed. The entity owns its own change detection and bumps
    /// <see cref="UpdatedAt"/> only when something really changed. A null
    /// <paramref name="profilePictureUrl"/> means "leave the picture as-is".
    /// </summary>
    public IReadOnlyList<FieldChange> ApplyUpdate(
        string firstName,
        string lastName,
        string email,
        string? mobileNumber,
        string? gender,
        int cityId,
        string? addressLine,
        DateOnly? dateOfBirth,
        string? profilePictureUrl)
    {
        var changes = new List<FieldChange>();

        if (FirstName != firstName) { changes.Add(new(nameof(FirstName), FirstName, firstName)); FirstName = firstName; }
        if (LastName != lastName) { changes.Add(new(nameof(LastName), LastName, lastName)); LastName = lastName; }
        if (Email != email) { changes.Add(new(nameof(Email), Email, email)); Email = email; }
        if (MobileNumber != mobileNumber) { changes.Add(new(nameof(MobileNumber), MobileNumber, mobileNumber)); MobileNumber = mobileNumber; }
        if (Gender != gender) { changes.Add(new(nameof(Gender), Gender, gender)); Gender = gender; }
        if (CityId != cityId) { changes.Add(new(nameof(CityId), CityId.ToString(), cityId.ToString())); CityId = cityId; }
        if (AddressLine != addressLine) { changes.Add(new(nameof(AddressLine), AddressLine, addressLine)); AddressLine = addressLine; }
        if (DateOfBirth != dateOfBirth) { changes.Add(new(nameof(DateOfBirth), DateOfBirth?.ToString(), dateOfBirth?.ToString())); DateOfBirth = dateOfBirth; }
        if (profilePictureUrl != null && ProfilePictureUrl != profilePictureUrl)
        {
            changes.Add(new(nameof(ProfilePictureUrl), ProfilePictureUrl, profilePictureUrl));
            ProfilePictureUrl = profilePictureUrl;
        }

        if (changes.Count > 0)
            UpdatedAt = DateTime.UtcNow;

        return changes;
    }

    /// <summary>
    /// Soft-deletes the person by deactivating it. Encapsulates the
    /// invariant that a soft-delete must also touch <see cref="UpdatedAt"/>.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
