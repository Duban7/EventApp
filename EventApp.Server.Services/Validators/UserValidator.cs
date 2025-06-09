using Data.Models;
using FluentValidation;
using FluentValidation.Results;
using Services.Exeptions;

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
                .NotEmpty()
                .Length(2, 30).WithMessage(lengthMsg);

            RuleFor(user => user.Surname)
                .NotEmpty()
                .Length(2, 50).WithMessage(lengthMsg);

            RuleFor(user => user.BirthDate)
                .NotEmpty().WithMessage(msg);
        }

        protected override void RaiseValidationException(ValidationContext<User> context, ValidationResult result)
        {
            var firstError = result.Errors[0];
            throw new BadRequestException(firstError.ErrorMessage);
        }
    }
}
