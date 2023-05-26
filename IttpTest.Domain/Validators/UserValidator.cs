using FluentValidation;
using IttpTest.Domain.Models;

namespace IttpTest.Domain.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Login).Matches("^[a-zA-Z0-9]*$")
            .WithMessage("Login must contain only english letters and numbers.");
        RuleFor(user => user.Password).Matches("^[a-zA-Z0-9]*$")
            .WithMessage("Password must contain only english letters and numbers.");
        RuleFor(user => user.Name).Matches("^[a-zA-Zа-яА-Я]*$")
            .WithMessage("Name must contain only english or cyrillic letters and numbers.");
        RuleFor(user => user.Gender).InclusiveBetween(0, 2)
            .WithMessage("Gender option is 0 for Female, 1 for Male and 2 for Undefined");
    }
}