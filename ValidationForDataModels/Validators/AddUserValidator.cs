using FluentValidation;
using ValidationForDataModels.DTO;

namespace ValidationForDataModels.Validators
{
    public class AddUserValidator : AbstractValidator<AddUser>
    {
        public AddUserValidator()
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(10).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(30).WithMessage("Email cannot exceed 30 characters.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(15).WithMessage("Password cannot exceed 50 characters.");
        }
    }
}