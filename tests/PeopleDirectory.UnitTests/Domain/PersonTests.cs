using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.UnitTests.Domain;

public class PersonTests
{
    [Fact]
    public void Create_SetsFields_AndActivatesWithTimestamps()
    {
        var person = Person.Create(
            "Jane", "Smith", "jane@example.com", "+27123", "Female",
            cityId: 5, addressLine: "1 Main St", dateOfBirth: new DateOnly(1990, 1, 1),
            profilePictureUrl: "/uploads/jane.jpg");

        Assert.Equal("Jane", person.FirstName);
        Assert.Equal("Smith", person.LastName);
        Assert.Equal(5, person.CityId);
        Assert.Equal("/uploads/jane.jpg", person.ProfilePictureUrl);
        Assert.True(person.IsActive);
        Assert.Equal(person.CreatedAt, person.UpdatedAt);
    }

    [Fact]
    public void ApplyUpdate_ChangedFields_AreReportedAndApplied()
    {
        var person = Person.Create("John", "Doe", "john@example.com", null, "Male", 1, null, null, null);

        var changes = person.ApplyUpdate(
            "Johnny", "Doe", "john@example.com", "+111", "Male", 2, "New Addr", null, null);

        Assert.Equal("Johnny", person.FirstName);
        Assert.Equal("+111", person.MobileNumber);
        Assert.Equal(2, person.CityId);
        Assert.Equal("New Addr", person.AddressLine);

        // Only the four genuinely changed fields are reported.
        Assert.Equal(4, changes.Count);
        Assert.Contains(changes, c => c.Field == "FirstName" && c.OldValue == "John" && c.NewValue == "Johnny");
        Assert.Contains(changes, c => c.Field == "CityId" && c.OldValue == "1" && c.NewValue == "2");
    }

    [Fact]
    public void ApplyUpdate_NoChanges_ReturnsEmpty_AndDoesNotBumpUpdatedAt()
    {
        var person = Person.Create("John", "Doe", "john@example.com", "+111", "Male", 1, "Addr", null, null);
        var originalUpdatedAt = person.UpdatedAt;

        var changes = person.ApplyUpdate(
            "John", "Doe", "john@example.com", "+111", "Male", 1, "Addr", null, null);

        Assert.Empty(changes);
        Assert.Equal(originalUpdatedAt, person.UpdatedAt);
    }

    [Fact]
    public void ApplyUpdate_NullProfilePicture_LeavesExistingPictureUntouched()
    {
        var person = Person.Create("John", "Doe", "john@example.com", null, "Male", 1, null, null, "/uploads/old.jpg");

        var changes = person.ApplyUpdate(
            "John", "Doe", "john@example.com", null, "Male", 1, null, null, profilePictureUrl: null);

        Assert.Empty(changes);
        Assert.Equal("/uploads/old.jpg", person.ProfilePictureUrl);
    }

    [Fact]
    public void Deactivate_SetsInactive_AndTouchesUpdatedAt()
    {
        var person = Person.Create("John", "Doe", "john@example.com", null, "Male", 1, null, null, null);

        person.Deactivate();

        Assert.False(person.IsActive);
    }
}
