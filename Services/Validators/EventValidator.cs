using Data.Models;
using FluentValidation;

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
                .Must(sd=>sd.Value>DateTime.Now).WithMessage(msg);
        }
    }
}
