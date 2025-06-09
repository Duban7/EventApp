using Data.Models;
using FluentValidation;
using FluentValidation.Results;
using Services.Exeptions;

namespace Services.Validators
{
    public class EventValidator : AbstractValidator<Event>
    {
        public EventValidator()
        {
            string msg = "Property cannot be null";
            string lengthMsg = "Invalid length for {PropertyName}";

            RuleFor(e => e.Name)
                .Length(4, 40).WithMessage(lengthMsg);

            RuleFor(e => e.StartDate)
                .NotEmpty().WithMessage(msg);

            RuleFor(e => e.Description)
                .NotEmpty().WithMessage(msg);

            RuleFor(e => e.Category)
                .NotEmpty().WithMessage(msg);

            RuleFor(e => e.EventPlace)
                .NotEmpty().WithMessage(msg);

            RuleFor(e => e.MaxParticipantsCount)
                .NotEmpty().WithMessage(msg);
        }
        protected override void RaiseValidationException(ValidationContext<Event> context, ValidationResult result)
        {
            var firstError = result.Errors[0];
            throw new BadRequestException(firstError.ErrorMessage);
        }
    }
}
