using Data.Models;
using FluentValidation;
using FluentValidation.Results;
using Services.Exeptions;
using System.Text.RegularExpressions;

namespace Services.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            string msg = "Error in property {PropertyName}: value {PropertyValue}";
            string lengthMsg = "Invalid length for {PropertyName}";

            RuleFor(user => user.Email)
                .EmailAddress().WithMessage(msg);

            RuleFor(user => user.Name)
                .Length(2, 30).WithMessage(lengthMsg);

            RuleFor(user => user.Surname)
                .Length(2, 50).WithMessage(lengthMsg);

            RuleFor(user => user.BirthDate)
                .Must(bd => bd.Value < DateTime.Now).WithMessage(msg);
        }

        public static bool IsPasswordValid(string password) =>
            Regex.IsMatch(password, @"(?=.*[0-9])(?=.*[A-Za-z])[0-9a-zA-Z_\-]{6,100}");

        protected override void RaiseValidationException(ValidationContext<User> context, ValidationResult result)
        {
            var firstError = result.Errors[0];
            throw new BadRequestException(firstError.ErrorMessage);
        }
    }
}
