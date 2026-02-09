using FluentValidation;
using GoogleLogin.DTO;

namespace GoogleLogin.Validators
{
    public class AddUserValidator : AbstractValidator<AddUser>
    {
        public AddUserValidator()
        {
            RuleFor(user => user.Username)
                .NotEmpty()
                .WithMessage("Name is required.")
                .WithErrorCode("not empty")
                .MinimumLength(3)
                .WithMessage("Name must be at least 3 characters long.")
                .WithErrorCode("minimum length")
                .MaximumLength(20)
                .WithMessage("Name cannot exceed 20 characters.")
                .WithErrorCode("maximum length");

            RuleFor(user => user.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .WithErrorCode("not empty")
                .EmailAddress()
                .WithMessage("A valid email is required.")
                .WithErrorCode("invalid email")
                .MaximumLength(30)
                .WithMessage("Email cannot exceed 30 characters.")
                .WithErrorCode("maximum length");

            RuleFor(user => user.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .WithErrorCode("not empty")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.")
                .WithErrorCode("minimum length")
                .MaximumLength(255)
                .WithMessage("Password cannot exceed 255 characters.")
                .WithErrorCode("maximum length");
        }
    }
}