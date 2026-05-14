using FluentValidation.TestHelper;
using PeopleDirectory.Application.DTOs;
using PeopleDirectory.Application.Validators;

namespace PeopleDirectory.UnitTests.Validators;

public class PersonCreateDtoValidatorTests
{
    private readonly PersonCreateDtoValidator _validator = new();

    [Fact]
    public void Valid_Dto_PassesValidation()
    {
        var dto = new PersonCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            CityId = 1,
            Gender = "Male"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void FirstName_Empty_FailsValidation(string? firstName)
    {
        var dto = new PersonCreateDto { FirstName = firstName!, LastName = "Doe", Email = "a@b.com", CityId = 1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void FirstName_TooLong_FailsValidation()
    {
        var dto = new PersonCreateDto
        {
            FirstName = new string('A', 101),
            LastName = "Doe",
            Email = "a@b.com",
            CityId = 1
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void LastName_Empty_FailsValidation(string? lastName)
    {
        var dto = new PersonCreateDto { FirstName = "John", LastName = lastName!, Email = "a@b.com", CityId = 1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Email_Invalid_FailsValidation(string email)
    {
        var dto = new PersonCreateDto { FirstName = "John", LastName = "Doe", Email = email, CityId = 1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Email_TooLong_FailsValidation()
    {
        var dto = new PersonCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = new string('a', 250) + "@b.com",
            CityId = 1
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CityId_Zero_FailsValidation()
    {
        var dto = new PersonCreateDto { FirstName = "John", LastName = "Doe", Email = "a@b.com", CityId = 0 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CityId);
    }

    [Fact]
    public void CityId_Negative_FailsValidation()
    {
        var dto = new PersonCreateDto { FirstName = "John", LastName = "Doe", Email = "a@b.com", CityId = -1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CityId);
    }

    [Theory]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("Other")]
    [InlineData(null)]
    public void Gender_ValidValues_PassValidation(string? gender)
    {
        var dto = new PersonCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "a@b.com",
            CityId = 1,
            Gender = gender
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public void Gender_InvalidValue_FailsValidation()
    {
        var dto = new PersonCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "a@b.com",
            CityId = 1,
            Gender = "InvalidGender"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public void MobileNumber_TooLong_FailsValidation()
    {
        var dto = new PersonCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "a@b.com",
            CityId = 1,
            MobileNumber = new string('1', 21)
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.MobileNumber);
    }

    [Fact]
    public void AddressLine_TooLong_FailsValidation()
    {
        var dto = new PersonCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "a@b.com",
            CityId = 1,
            AddressLine = new string('A', 301)
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.AddressLine);
    }
}

public class PersonUpdateDtoValidatorTests
{
    private readonly PersonUpdateDtoValidator _validator = new();

    [Fact]
    public void Valid_Dto_PassesValidation()
    {
        var dto = new PersonUpdateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            CityId = 1,
            Gender = "Male"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void FirstName_Empty_FailsValidation()
    {
        var dto = new PersonUpdateDto { FirstName = "", LastName = "Doe", Email = "a@b.com", CityId = 1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Email_Invalid_FailsValidation()
    {
        var dto = new PersonUpdateDto { FirstName = "John", LastName = "Doe", Email = "bad", CityId = 1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CityId_Zero_FailsValidation()
    {
        var dto = new PersonUpdateDto { FirstName = "John", LastName = "Doe", Email = "a@b.com", CityId = 0 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CityId);
    }

    [Fact]
    public void Gender_InvalidValue_FailsValidation()
    {
        var dto = new PersonUpdateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "a@b.com",
            CityId = 1,
            Gender = "Unknown"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Gender);
    }
}

public class LoginDtoValidatorTests
{
    private readonly LoginDtoValidator _validator = new();

    [Fact]
    public void Valid_Login_PassesValidation()
    {
        var dto = new LoginDto { Email = "admin@test.com", Password = "Admin@123" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Email_Empty_FailsValidation()
    {
        var dto = new LoginDto { Email = "", Password = "Admin@123" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Email_InvalidFormat_FailsValidation()
    {
        var dto = new LoginDto { Email = "not-email", Password = "Admin@123" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Password_Empty_FailsValidation()
    {
        var dto = new LoginDto { Email = "admin@test.com", Password = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_TooShort_FailsValidation()
    {
        var dto = new LoginDto { Email = "admin@test.com", Password = "12345" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
