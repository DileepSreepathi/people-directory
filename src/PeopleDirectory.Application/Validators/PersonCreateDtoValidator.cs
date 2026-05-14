using FluentValidation;
using PeopleDirectory.Application.DTOs;

namespace PeopleDirectory.Application.Validators;

public class PersonCreateDtoValidator : AbstractValidator<PersonCreateDto>
{
    public PersonCreateDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.MobileNumber).MaximumLength(20);
        RuleFor(x => x.Gender).Must(g => g == null || g == "Male" || g == "Female" || g == "Other")
            .WithMessage("Gender must be Male, Female, or Other.");
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.AddressLine).MaximumLength(300);
    }
}
