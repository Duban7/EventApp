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
            string msg = "Error in property {PropertyName}: value {PropertyValue}";
            string lengthMsg = "Invalid length for {PropertyName}";

            RuleFor(e => e.Name)
                .Length(4, 40).WithMessage(lengthMsg);

            RuleFor(e => e.StartDate)
                .Must(sd => sd.Value > DateTime.Now).WithMessage(msg);

            RuleFor(e => e.MaxParticipantsCount)
                .Must(mp => 3 < mp && mp < 300).WithMessage(msg);
        }
        protected override void RaiseValidationException(ValidationContext<Event> context, ValidationResult result)
        {
            var firstError = result.Errors[0];
            throw new BadRequestException(firstError.ErrorMessage);
        }
    }
}
