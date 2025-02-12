using EventTicketingManagementSystem.Request;
using FluentValidation;

namespace EventTicketingManagementSystem.Validators
{
    public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
    {
        public UpdateUserProfileRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must be less than 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10,15}$").WithMessage("Phone number must be between 10 and 15 digits.");

            When(x => x.AllowChangePassword, () =>
            {
                RuleFor(x => x.OldPassword)
                    .NotEmpty().WithMessage("Old password is required.");

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage("New password is required.")
                    .MinimumLength(6).WithMessage("New password must be at least 6 characters long.");

                RuleFor(x => x.ConfirmedNewPassword)
                    .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
            });
        }
    }
}
