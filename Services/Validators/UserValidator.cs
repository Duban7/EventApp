using Data.Models;
using FluentValidation;

namespace Services.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            string msg = "Error in property {PropertyName}: value {PropertyValue}";
            string lengthMsg = "Invalid length for {PropertyName}";

            RuleFor(user => user.Name)
                .Length(2, 30).WithMessage(lengthMsg);

            RuleFor(user => user.Surname)
                .Length(2, 50).WithMessage(lengthMsg);

            RuleFor(user => user.BirthDate)
                .Must(bd => bd.Value < DateTime.Now).WithMessage(msg);

        }
    }
}
